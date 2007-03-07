namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class IoAdaptedObjectContainer : Db4objects.Db4o.Internal.LocalObjectContainer
	{
		private readonly string _fileName;

		private Db4objects.Db4o.IO.IoAdapter _file;

		private Db4objects.Db4o.IO.IoAdapter _timerFile;

		private volatile Db4objects.Db4o.IO.IoAdapter _backupFile;

		private object _fileLock;

		private readonly Db4objects.Db4o.Config.IFreespaceFiller _freespaceFiller;

		internal IoAdaptedObjectContainer(Db4objects.Db4o.Config.IConfiguration config, string
			 fileName) : base(config, null)
		{
			lock (i_lock)
			{
				_fileLock = new object();
				_fileName = fileName;
				_freespaceFiller = CreateFreespaceFiller();
				try
				{
					Open();
				}
				catch (Db4objects.Db4o.Ext.DatabaseFileLockedException e)
				{
					StopSession();
					throw;
				}
				Initialize3();
			}
		}

		public override void Backup(string path)
		{
			lock (i_lock)
			{
				CheckClosed();
				if (_backupFile != null)
				{
					Db4objects.Db4o.Internal.Exceptions4.ThrowRuntimeException(61);
				}
				try
				{
					_backupFile = ConfigImpl().IoAdapter().Open(path, true, _file.GetLength());
					_backupFile.BlockSize(BlockSize());
				}
				catch
				{
					_backupFile = null;
					Db4objects.Db4o.Internal.Exceptions4.ThrowRuntimeException(12, path);
				}
			}
			long pos = 0;
			int bufferlength = 8192;
			byte[] buffer = new byte[bufferlength];
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
				try
				{
					Sharpen.Lang.Thread.Sleep(1);
				}
				catch
				{
				}
			}
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

		protected override void Close2()
		{
			FreePrefetchedPointers();
			Write(true);
			base.Close2();
			lock (_fileLock)
			{
				try
				{
					_file.Close();
					_file = null;
					_fileHeader.Close();
					CloseTimerFile();
				}
				catch (System.Exception e)
				{
					_file = null;
					Db4objects.Db4o.Internal.Exceptions4.ThrowRuntimeException(11, e);
				}
				_file = null;
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
			if (Db4objects.Db4o.Debug.xbytes && Db4objects.Db4o.Deploy.overwrite)
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
			catch (System.Exception e)
			{
				Db4objects.Db4o.Internal.Exceptions4.ThrowRuntimeException(16, e);
			}
		}

		private void CheckXBytes(int newAddress, int newAddressOffset, int length)
		{
			if (Db4objects.Db4o.Debug.xbytes && Db4objects.Db4o.Deploy.overwrite)
			{
				try
				{
					byte[] checkXBytes = new byte[length];
					_file.BlockSeek(newAddress, newAddressOffset);
					_file.Read(checkXBytes);
					for (int i = 0; i < checkXBytes.Length; i++)
					{
						if (checkXBytes[i] != Db4objects.Db4o.Internal.Const4.XBYTE)
						{
							string msg = "XByte corruption adress:" + newAddress + " length:" + length;
							throw new System.Exception(msg);
						}
					}
				}
				catch (System.Exception e)
				{
					Sharpen.Runtime.PrintStackTrace(e);
				}
			}
		}

		internal override void EmergencyClose()
		{
			base.EmergencyClose();
			try
			{
				_file.Close();
			}
			catch
			{
			}
			_file = null;
		}

		public override long FileLength()
		{
			try
			{
				return _file.GetLength();
			}
			catch
			{
				throw new System.Exception();
			}
		}

		internal override string FileName()
		{
			return _fileName;
		}

		private void Open()
		{
			bool isNew = false;
			Db4objects.Db4o.IO.IoAdapter ioAdapter = ConfigImpl().IoAdapter();
			try
			{
				if (FileName().Length > 0)
				{
					if (!ioAdapter.Exists(FileName()))
					{
						isNew = true;
						LogMsg(14, FileName());
						i_handlers.OldEncryptionOff();
					}
					try
					{
						bool lockFile = Db4objects.Db4o.Debug.lockFile && ConfigImpl().LockFile() && (!ConfigImpl
							().IsReadOnly());
						_file = ioAdapter.Open(FileName(), lockFile, 0);
						if (NeedsTimerFile())
						{
							_timerFile = ioAdapter.Open(FileName(), false, 0);
						}
					}
					catch (Db4objects.Db4o.Ext.DatabaseFileLockedException de)
					{
						throw;
					}
					catch (System.Exception e)
					{
						Db4objects.Db4o.Internal.Exceptions4.ThrowRuntimeException(12, FileName(), e);
					}
					if (isNew)
					{
						ConfigureNewFile();
						if (ConfigImpl().ReservedStorageSpace() > 0)
						{
							Reserve(ConfigImpl().ReservedStorageSpace());
						}
						Write(false);
						WriteHeader(true, false);
					}
					else
					{
						ReadThis();
					}
				}
				else
				{
					Db4objects.Db4o.Internal.Exceptions4.ThrowRuntimeException(21);
				}
			}
			catch (System.Exception exc)
			{
				if (i_references != null)
				{
					i_references.StopTimer();
				}
				throw;
			}
		}

		public override void ReadBytes(byte[] bytes, int address, int length)
		{
			ReadBytes(bytes, address, 0, length);
		}

		public override void ReadBytes(byte[] bytes, int address, int addressOffset, int 
			length)
		{
			try
			{
				_file.BlockSeek(address, addressOffset);
				int bytesRead = _file.Read(bytes, length);
				AssertRead(bytesRead, length);
			}
			catch (System.IO.IOException ioex)
			{
				throw new Db4objects.Db4o.IO.UncheckedIOException(ioex);
			}
		}

		private void AssertRead(int bytesRead, int expected)
		{
			if (bytesRead != expected)
			{
				throw new Db4objects.Db4o.IO.UncheckedIOException("expected = " + expected + ", read = "
					 + bytesRead);
			}
		}

		internal override void Reserve(int byteCount)
		{
			lock (i_lock)
			{
				int address = GetSlot(byteCount);
				ZeroReservedStorage(address, byteCount);
				Free(address, byteCount);
			}
		}

		private void ZeroReservedStorage(int address, int length)
		{
			if (ConfigImpl().IsReadOnly())
			{
				return;
			}
			try
			{
				ZeroFile(_file, address, length);
				ZeroFile(_backupFile, address, length);
			}
			catch (System.IO.IOException e)
			{
				Db4objects.Db4o.Internal.Exceptions4.ThrowRuntimeException(16, e);
			}
		}

		private void ZeroFile(Db4objects.Db4o.IO.IoAdapter io, int address, int length)
		{
			if (io == null)
			{
				return;
			}
			byte[] zeroBytes = new byte[1024];
			int left = length;
			io.BlockSeek(address, 0);
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
			catch
			{
			}
		}

		private bool NeedsTimerFile()
		{
			return NeedsLockFileThread() && Db4objects.Db4o.Debug.lockFile;
		}

		private void CloseTimerFile()
		{
			if (_timerFile == null)
			{
				return;
			}
			_timerFile.Close();
			_timerFile = null;
		}

		public override void WriteBytes(Db4objects.Db4o.Internal.Buffer bytes, int address
			, int addressOffset)
		{
			if (ConfigImpl().IsReadOnly())
			{
				return;
			}
			if (Db4objects.Db4o.Deploy.debug && !Db4objects.Db4o.Deploy.flush)
			{
				return;
			}
			try
			{
				if (Db4objects.Db4o.Debug.xbytes && Db4objects.Db4o.Deploy.overwrite)
				{
					bool doCheck = true;
					if (bytes is Db4objects.Db4o.Internal.StatefulBuffer)
					{
						Db4objects.Db4o.Internal.StatefulBuffer writer = (Db4objects.Db4o.Internal.StatefulBuffer
							)bytes;
						if (writer.GetID() == Db4objects.Db4o.Internal.Const4.IGNORE_ID)
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
			catch (System.Exception e)
			{
				Db4objects.Db4o.Internal.Exceptions4.ThrowRuntimeException(16, e);
			}
		}

		public override void OverwriteDeletedBytes(int address, int length)
		{
			if (!ConfigImpl().IsReadOnly() && _freespaceFiller != null)
			{
				if (address > 0 && length > 0)
				{
					Db4objects.Db4o.IO.IoAdapterWindow window = new Db4objects.Db4o.IO.IoAdapterWindow
						(_file, address, length);
					try
					{
						CreateFreespaceFiller().Fill(window);
					}
					catch (System.Exception e)
					{
						Sharpen.Runtime.PrintStackTrace(e);
					}
					finally
					{
						window.Disable();
					}
				}
			}
		}

		public virtual Db4objects.Db4o.IO.IoAdapter TimerFile()
		{
			return _timerFile;
		}

		private Db4objects.Db4o.Config.IFreespaceFiller CreateFreespaceFiller()
		{
			Db4objects.Db4o.Config.IFreespaceFiller freespaceFiller = Config().FreespaceFiller
				();
			return freespaceFiller;
		}

		private class XByteFreespaceFiller : Db4objects.Db4o.Config.IFreespaceFiller
		{
			public virtual void Fill(Db4objects.Db4o.IO.IoAdapterWindow io)
			{
				io.Write(0, XBytes(io.Length()));
			}

			private byte[] XBytes(int len)
			{
				byte[] bytes = new byte[len];
				for (int i = 0; i < len; i++)
				{
					bytes[i] = Db4objects.Db4o.Internal.Const4.XBYTE;
				}
				return bytes;
			}
		}
	}
}
