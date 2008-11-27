/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class IoAdaptedObjectContainer : LocalObjectContainer
	{
		private readonly string _fileName;

		private BlockAwareBin _file;

		private BlockAwareBin _timerFile;

		private volatile BlockAwareBin _backupFile;

		private object _fileLock;

		private readonly IFreespaceFiller _freespaceFiller;

		/// <exception cref="OldFormatException"></exception>
		internal IoAdaptedObjectContainer(IConfiguration config, string fileName) : base(
			config, null)
		{
			//This is necessary as a separate File because access is not synchronized with access for normal data read/write so the seek pointer can get lost.
			_fileLock = new object();
			_fileName = fileName;
			_freespaceFiller = CreateFreespaceFiller();
			Open();
		}

		/// <exception cref="OldFormatException"></exception>
		/// <exception cref="DatabaseReadOnlyException"></exception>
		protected sealed override void OpenImpl()
		{
			IStorage storage = ConfigImpl().Storage;
			bool isNew = !storage.Exists(FileName());
			if (isNew)
			{
				LogMsg(14, FileName());
				CheckReadOnly();
				_handlers.OldEncryptionOff();
			}
			bool readOnly = ConfigImpl().IsReadOnly();
			bool lockFile = Debug.lockFile && ConfigImpl().LockFile() && (!readOnly);
			_file = new BlockAwareBin(storage.Open(FileName(), lockFile, 0, readOnly));
			if (NeedsLockFileThread())
			{
				// FIXME: with a SynchronizedStorage or something
				_timerFile = new BlockAwareBin(storage.Open(FileName(), false, 0, false));
			}
			if (isNew)
			{
				ConfigureNewFile();
				if (ConfigImpl().ReservedStorageSpace() > 0)
				{
					Reserve(ConfigImpl().ReservedStorageSpace());
				}
				CommitTransaction();
				WriteHeader(true, false);
			}
			else
			{
				ReadThis();
			}
		}

		/// <exception cref="DatabaseClosedException"></exception>
		/// <exception cref="Db4oIOException"></exception>
		public override void Backup(string path)
		{
			lock (_lock)
			{
				CheckClosed();
				if (_backupFile != null)
				{
					throw new BackupInProgressException();
				}
				_backupFile = new BlockAwareBin(ConfigImpl().Storage.Open(path, true, _file.Length
					(), false));
				_backupFile.BlockSize(BlockSize());
			}
			long pos = 0;
			byte[] buffer = new byte[8192];
			while (true)
			{
				lock (_lock)
				{
					int read = _file.Read(pos, buffer);
					if (read <= 0)
					{
						break;
					}
					_backupFile.Write(pos, buffer, read);
					pos += read;
				}
			}
			Cool.SleepIgnoringInterruption(1);
			lock (_lock)
			{
				_backupFile.Close();
				_backupFile = null;
			}
		}

		public override void BlockSize(int size)
		{
			_file.BlockSize(size);
			if (_timerFile != null)
			{
				_timerFile.BlockSize(size);
			}
		}

		public override byte BlockSize()
		{
			return (byte)_file.BlockSize();
		}

		protected override void FreeInternalResources()
		{
			FreePrefetchedPointers();
		}

		protected override void ShutdownDataStorage()
		{
			lock (_fileLock)
			{
				CloseDatabaseFile();
				CloseFileHeader();
				CloseTimerFile();
			}
		}

		private void CloseDatabaseFile()
		{
			try
			{
				if (_file != null)
				{
					_file.Close();
				}
			}
			finally
			{
				_file = null;
			}
		}

		private void CloseFileHeader()
		{
			try
			{
				if (_fileHeader != null)
				{
					_fileHeader.Close();
				}
			}
			finally
			{
				_fileHeader = null;
			}
		}

		protected override void CloseSystemTransaction()
		{
			((LocalTransaction)SystemTransaction()).Close();
		}

		private void CloseTimerFile()
		{
			try
			{
				if (_timerFile != null)
				{
					_timerFile.Close();
				}
			}
			finally
			{
				_timerFile = null;
			}
		}

		public override void Commit1(Transaction trans)
		{
			EnsureLastSlotWritten();
			base.Commit1(trans);
		}

		public override void Copy(int oldAddress, int oldAddressOffset, int newAddress, int
			 newAddressOffset, int length)
		{
			if (Debug.xbytes && Deploy.overwrite)
			{
				CheckXBytes(newAddress, newAddressOffset, length);
			}
			try
			{
				if (_backupFile == null)
				{
					_file.BlockCopy(oldAddress, oldAddressOffset, newAddress, newAddressOffset, length
						);
					return;
				}
				byte[] copyBytes = new byte[length];
				_file.BlockRead(oldAddress, oldAddressOffset, copyBytes);
				_file.BlockWrite(newAddress, newAddressOffset, copyBytes);
				if (_backupFile != null)
				{
					_backupFile.BlockWrite(newAddress, newAddressOffset, copyBytes);
				}
			}
			catch (Exception e)
			{
				Exceptions4.ThrowRuntimeException(16, e);
			}
		}

		private void CheckXBytes(int newAddress, int newAddressOffset, int length)
		{
			if (Debug.xbytes && Deploy.overwrite)
			{
				try
				{
					byte[] checkXBytes = new byte[length];
					_file.BlockRead(newAddress, newAddressOffset, checkXBytes);
					for (int i = 0; i < checkXBytes.Length; i++)
					{
						if (checkXBytes[i] != Const4.Xbyte)
						{
							string msg = "XByte corruption adress:" + newAddress + " length:" + length + " starting:"
								 + i;
							throw new Db4oException(msg);
						}
					}
				}
				catch (Exception e)
				{
					Sharpen.Runtime.PrintStackTrace(e);
				}
			}
		}

		public override long FileLength()
		{
			try
			{
				return _file.Length();
			}
			catch (Exception)
			{
				throw new Exception();
			}
		}

		public override string FileName()
		{
			return _fileName;
		}

		/// <exception cref="Db4oIOException"></exception>
		public override void ReadBytes(byte[] bytes, int address, int length)
		{
			ReadBytes(bytes, address, 0, length);
		}

		/// <exception cref="Db4oIOException"></exception>
		public override void ReadBytes(byte[] bytes, int address, int addressOffset, int 
			length)
		{
			if (DTrace.enabled)
			{
				DTrace.ReadBytes.LogLength(address + addressOffset, length);
			}
			int bytesRead = _file.BlockRead(address, addressOffset, bytes, length);
			CheckReadCount(bytesRead, length);
		}

		private void CheckReadCount(int bytesRead, int expected)
		{
			if (bytesRead != expected)
			{
				throw new IncompatibleFileFormatException();
			}
		}

		/// <exception cref="DatabaseReadOnlyException"></exception>
		public override void Reserve(int byteCount)
		{
			CheckReadOnly();
			lock (_lock)
			{
				Slot slot = GetSlot(byteCount);
				ZeroReservedSlot(slot);
				Free(slot);
			}
		}

		private void ZeroReservedSlot(Slot slot)
		{
			ZeroFile(_file, slot);
			ZeroFile(_backupFile, slot);
		}

		private void ZeroFile(BlockAwareBin io, Slot slot)
		{
			if (io == null)
			{
				return;
			}
			byte[] zeroBytes = new byte[1024];
			int left = slot.Length();
			int offset = 0;
			while (left > zeroBytes.Length)
			{
				io.BlockWrite(slot.Address(), offset, zeroBytes, zeroBytes.Length);
				offset += zeroBytes.Length;
				left -= zeroBytes.Length;
			}
			if (left > 0)
			{
				io.BlockWrite(slot.Address(), offset, zeroBytes, left);
			}
		}

		public override void SyncFiles()
		{
			_file.Sync();
			if (_timerFile != null)
			{
				// _timerFile can be set to null here by other thread
				try
				{
					_timerFile.Sync();
				}
				catch (Exception)
				{
				}
			}
		}

		public override void WriteBytes(ByteArrayBuffer buffer, int blockedAddress, int addressOffset
			)
		{
			if (Deploy.debug && !Deploy.flush)
			{
				return;
			}
			if (Debug.xbytes && Deploy.overwrite)
			{
				bool doCheck = true;
				if (buffer is StatefulBuffer)
				{
					StatefulBuffer writer = (StatefulBuffer)buffer;
					if (writer.GetID() == Const4.IgnoreId)
					{
						doCheck = false;
					}
				}
				if (doCheck)
				{
					CheckXBytes(blockedAddress, addressOffset, buffer.Length());
				}
			}
			if (DTrace.enabled)
			{
				DTrace.WriteBytes.LogLength(blockedAddress + addressOffset, buffer.Length());
			}
			_file.BlockWrite(blockedAddress, addressOffset, buffer._buffer, buffer.Length());
			if (_backupFile != null)
			{
				_backupFile.BlockWrite(blockedAddress, addressOffset, buffer._buffer, buffer.Length
					());
			}
		}

		public override void OverwriteDeletedBytes(int address, int length)
		{
			if (_freespaceFiller == null)
			{
				return;
			}
			if (address > 0 && length > 0)
			{
				if (DTrace.enabled)
				{
					DTrace.WriteXbytes.LogLength(address, length);
				}
				BlockAwareBinWindow window = new BlockAwareBinWindow(_file, address, length);
				try
				{
					CreateFreespaceFiller().Fill(window);
				}
				catch (IOException e)
				{
					Sharpen.Runtime.PrintStackTrace(e);
				}
				finally
				{
					window.Disable();
				}
			}
		}

		public virtual BlockAwareBin TimerFile()
		{
			return _timerFile;
		}

		private IFreespaceFiller CreateFreespaceFiller()
		{
			IFreespaceFiller freespaceFiller = Config().FreespaceFiller();
			return freespaceFiller;
		}

		private class XByteFreespaceFiller : IFreespaceFiller
		{
			/// <exception cref="IOException"></exception>
			public virtual void Fill(BlockAwareBinWindow io)
			{
				io.Write(0, XBytes(io.Length()));
			}

			private byte[] XBytes(int len)
			{
				byte[] bytes = new byte[len];
				for (int i = 0; i < len; i++)
				{
					bytes[i] = Const4.Xbyte;
				}
				return bytes;
			}
		}
	}
}
