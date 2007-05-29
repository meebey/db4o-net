/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

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

		private IoAdapter _file;

		private IoAdapter _timerFile;

		private volatile IoAdapter _backupFile;

		private object _fileLock;

		private readonly IFreespaceFiller _freespaceFiller;

		internal IoAdaptedObjectContainer(IConfiguration config, string fileName) : base(
			config, null)
		{
			_fileLock = new object();
			_fileName = fileName;
			_freespaceFiller = CreateFreespaceFiller();
			Open();
		}

		protected sealed override void OpenImpl()
		{
			IoAdapter ioAdapter = ConfigImpl().IoAdapter();
			bool isNew = !ioAdapter.Exists(FileName());
			if (isNew)
			{
				LogMsg(14, FileName());
				CheckReadOnly();
				i_handlers.OldEncryptionOff();
			}
			bool lockFile = Debug.lockFile && ConfigImpl().LockFile() && (!ConfigImpl().IsReadOnly
				());
			_file = ioAdapter.Open(FileName(), lockFile, 0);
			if (NeedsTimerFile())
			{
				_timerFile = ioAdapter.DelegatedIoAdapter().Open(FileName(), false, 0);
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

		public override void Backup(string path)
		{
			lock (i_lock)
			{
				CheckClosed();
				if (_backupFile != null)
				{
					throw new BackupInProgressException();
				}
				_backupFile = ConfigImpl().IoAdapter().Open(path, true, _file.GetLength());
				_backupFile.BlockSize(BlockSize());
			}
			long pos = 0;
			byte[] buffer = new byte[8192];
			while (true)
			{
				lock (i_lock)
				{
					_file.Seek(pos);
					int read = _file.Read(buffer);
					if (read <= 0)
					{
						break;
					}
					_backupFile.Seek(pos);
					_backupFile.Write(buffer, read);
					pos += read;
				}
			}
			Cool.SleepIgnoringInterruption(1);
			lock (i_lock)
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

		public override void Commit1()
		{
			EnsureLastSlotWritten();
			base.Commit1();
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
				_file.BlockSeek(oldAddress, oldAddressOffset);
				_file.Read(copyBytes);
				_file.BlockSeek(newAddress, newAddressOffset);
				_file.Write(copyBytes);
				if (_backupFile != null)
				{
					_backupFile.BlockSeek(newAddress, newAddressOffset);
					_backupFile.Write(copyBytes);
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
					_file.BlockSeek(newAddress, newAddressOffset);
					_file.Read(checkXBytes);
					for (int i = 0; i < checkXBytes.Length; i++)
					{
						if (checkXBytes[i] != Const4.XBYTE)
						{
							string msg = "XByte corruption adress:" + newAddress + " length:" + length + " starting:"
								 + i;
							throw new Exception(msg);
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
				return _file.GetLength();
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

		public override void ReadBytes(byte[] bytes, int address, int length)
		{
			ReadBytes(bytes, address, 0, length);
		}

		public override void ReadBytes(byte[] bytes, int address, int addressOffset, int 
			length)
		{
			_file.BlockSeek(address, addressOffset);
			int bytesRead = _file.Read(bytes, length);
			CheckReadCount(bytesRead, length);
		}

		private void CheckReadCount(int bytesRead, int expected)
		{
			if (bytesRead != expected)
			{
				throw new IncompatibleFileFormatException();
			}
		}

		public override void Reserve(int byteCount)
		{
			CheckReadOnly();
			lock (i_lock)
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

		private void ZeroFile(IoAdapter io, Slot slot)
		{
			if (io == null)
			{
				return;
			}
			byte[] zeroBytes = new byte[1024];
			int left = slot.Length();
			io.BlockSeek(slot.Address(), 0);
			while (left > zeroBytes.Length)
			{
				io.Write(zeroBytes, zeroBytes.Length);
				left -= zeroBytes.Length;
			}
			if (left > 0)
			{
				io.Write(zeroBytes, left);
			}
		}

		public override void SyncFiles()
		{
			try
			{
				_file.Sync();
				if (_timerFile != null)
				{
					_timerFile.Sync();
				}
			}
			catch (Exception)
			{
			}
		}

		private bool NeedsTimerFile()
		{
			return NeedsLockFileThread() && Debug.lockFile;
		}

		public override void WriteBytes(Db4objects.Db4o.Internal.Buffer bytes, int address
			, int addressOffset)
		{
			if (Deploy.debug && !Deploy.flush)
			{
				return;
			}
			if (Debug.xbytes && Deploy.overwrite)
			{
				bool doCheck = true;
				if (bytes is StatefulBuffer)
				{
					StatefulBuffer writer = (StatefulBuffer)bytes;
					if (writer.GetID() == Const4.IGNORE_ID)
					{
						doCheck = false;
					}
				}
				if (doCheck)
				{
					CheckXBytes(address, addressOffset, bytes.GetLength());
				}
			}
			_file.BlockSeek(address, addressOffset);
			_file.Write(bytes._buffer, bytes.GetLength());
			if (_backupFile != null)
			{
				_backupFile.BlockSeek(address, addressOffset);
				_backupFile.Write(bytes._buffer, bytes.GetLength());
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
				IoAdapterWindow window = new IoAdapterWindow(_file, address, length);
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

		public virtual IoAdapter TimerFile()
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
			public virtual void Fill(IoAdapterWindow io)
			{
				io.Write(0, XBytes(io.Length()));
			}

			private byte[] XBytes(int len)
			{
				byte[] bytes = new byte[len];
				for (int i = 0; i < len; i++)
				{
					bytes[i] = Const4.XBYTE;
				}
				return bytes;
			}
		}
	}
}
