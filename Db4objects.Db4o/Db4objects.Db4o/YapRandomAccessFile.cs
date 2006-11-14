namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public class YapRandomAccessFile : Db4objects.Db4o.YapFile
	{
		private Db4objects.Db4o.Session i_session;

		private Db4objects.Db4o.IO.IoAdapter i_file;

		private Db4objects.Db4o.IO.IoAdapter i_timerFile;

		private volatile Db4objects.Db4o.IO.IoAdapter i_backupFile;

		private byte[] i_timerBytes = new byte[Db4objects.Db4o.YapConst.LONG_BYTES];

		private object i_fileLock;

		internal YapRandomAccessFile(Db4objects.Db4o.Config.IConfiguration config, Db4objects.Db4o.Session
			 a_session) : base(config, null)
		{
			lock (i_lock)
			{
				i_fileLock = new object();
				i_session = a_session;
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
				if (i_backupFile != null)
				{
					Db4objects.Db4o.Inside.Exceptions4.ThrowRuntimeException(61);
				}
				try
				{
					i_backupFile = ConfigImpl().IoAdapter().Open(path, true, i_file.GetLength());
					i_backupFile.BlockSize(BlockSize());
				}
				catch
				{
					i_backupFile = null;
					Db4objects.Db4o.Inside.Exceptions4.ThrowRuntimeException(12, path);
				}
			}
			long pos = 0;
			int bufferlength = 8192;
			byte[] buffer = new byte[bufferlength];
			while (true)
			{
				lock (i_lock)
				{
					i_file.Seek(pos);
					int read = i_file.Read(buffer);
					if (read <= 0)
					{
						break;
					}
					i_backupFile.Seek(pos);
					i_backupFile.Write(buffer, read);
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
				i_backupFile.Close();
				i_backupFile = null;
			}
		}

		public override void BlockSize(int size)
		{
			i_file.BlockSize(size);
			if (i_timerFile != null)
			{
				i_timerFile.BlockSize(size);
			}
		}

		public override byte BlockSize()
		{
			return (byte)i_file.BlockSize();
		}

		protected override bool Close2()
		{
			bool stopSession = true;
			lock (Db4objects.Db4o.Inside.Global4.Lock)
			{
				stopSession = i_session.CloseInstance();
				if (stopSession)
				{
					FreePrefetchedPointers();
					i_entryCounter++;
					try
					{
						Write(true);
					}
					catch (System.Exception t)
					{
						FatalException(t);
					}
					base.Close2();
					i_entryCounter--;
					Db4objects.Db4o.Db4oFactory.SessionStopped(i_session);
					lock (i_fileLock)
					{
						try
						{
							i_file.Close();
							i_file = null;
							_fileHeader.Close();
							CloseTimerFile();
						}
						catch (System.Exception e)
						{
							i_file = null;
							Db4objects.Db4o.Inside.Exceptions4.ThrowRuntimeException(11, e);
						}
						i_file = null;
					}
				}
			}
			return stopSession;
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
				if (i_backupFile == null)
				{
					i_file.BlockCopy(oldAddress, oldAddressOffset, newAddress, newAddressOffset, length
						);
					return;
				}
				byte[] copyBytes = new byte[length];
				i_file.BlockSeek(oldAddress, oldAddressOffset);
				i_file.Read(copyBytes);
				i_file.BlockSeek(newAddress, newAddressOffset);
				i_file.Write(copyBytes);
				if (i_backupFile != null)
				{
					i_backupFile.BlockSeek(newAddress, newAddressOffset);
					i_backupFile.Write(copyBytes);
				}
			}
			catch (System.Exception e)
			{
				Db4objects.Db4o.Inside.Exceptions4.ThrowRuntimeException(16, e);
			}
		}

		private void CheckXBytes(int a_newAddress, int newAddressOffset, int a_length)
		{
			if (Db4objects.Db4o.Debug.xbytes && Db4objects.Db4o.Deploy.overwrite)
			{
				try
				{
					byte[] checkXBytes = new byte[a_length];
					i_file.BlockSeek(a_newAddress, newAddressOffset);
					i_file.Read(checkXBytes);
					for (int i = 0; i < checkXBytes.Length; i++)
					{
						if (checkXBytes[i] != Db4objects.Db4o.YapConst.XBYTE)
						{
							string msg = "XByte corruption adress:" + a_newAddress + " length:" + a_length;
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
				i_file.Close();
			}
			catch
			{
			}
			try
			{
				Db4objects.Db4o.Db4oFactory.SessionStopped(i_session);
			}
			catch
			{
			}
			i_file = null;
		}

		public override long FileLength()
		{
			try
			{
				return i_file.GetLength();
			}
			catch
			{
				throw new System.Exception();
			}
		}

		internal override string FileName()
		{
			return i_session.FileName();
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
						i_file = ioAdapter.Open(FileName(), lockFile, 0);
						if (NeedsTimerFile())
						{
							i_timerFile = ioAdapter.Open(FileName(), false, 0);
						}
					}
					catch (Db4objects.Db4o.Ext.DatabaseFileLockedException de)
					{
						throw;
					}
					catch (System.Exception e)
					{
						Db4objects.Db4o.Inside.Exceptions4.ThrowRuntimeException(12, FileName(), e);
					}
					if (isNew)
					{
						ConfigureNewFile();
						if (ConfigImpl().ReservedStorageSpace() > 0)
						{
							Reserve(ConfigImpl().ReservedStorageSpace());
						}
						Write(false);
						WriteHeader(false);
					}
					else
					{
						ReadThis();
					}
				}
				else
				{
					Db4objects.Db4o.Inside.Exceptions4.ThrowRuntimeException(21);
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
				i_file.BlockSeek(address, addressOffset);
				i_file.Read(bytes, length);
			}
			catch (System.IO.IOException ioex)
			{
				throw new System.Exception();
			}
		}

		internal override void Reserve(int byteCount)
		{
			lock (i_lock)
			{
				int address = GetSlot(byteCount);
				WriteBytes(new Db4objects.Db4o.YapReader(byteCount), address, 0);
				Free(address, byteCount);
			}
		}

		public override void SyncFiles()
		{
			try
			{
				i_file.Sync();
				if (i_timerFile != null)
				{
					i_timerFile.Sync();
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

		public override bool WriteAccessTime(int address, int offset, long time)
		{
			lock (i_fileLock)
			{
				if (i_timerFile == null)
				{
					return false;
				}
				i_timerFile.BlockSeek(address, offset);
				Db4objects.Db4o.YLong.WriteLong(time, i_timerBytes);
				i_timerFile.Write(i_timerBytes);
				if (i_file == null)
				{
					CloseTimerFile();
					return false;
				}
				return true;
			}
		}

		private void CloseTimerFile()
		{
			if (i_timerFile == null)
			{
				return;
			}
			i_timerFile.Close();
			i_timerFile = null;
		}

		public override void WriteBytes(Db4objects.Db4o.YapReader a_bytes, int address, int
			 addressOffset)
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
					if (a_bytes is Db4objects.Db4o.YapWriter)
					{
						Db4objects.Db4o.YapWriter writer = (Db4objects.Db4o.YapWriter)a_bytes;
						if (writer.GetID() == Db4objects.Db4o.YapConst.IGNORE_ID)
						{
							doCheck = false;
						}
					}
					if (doCheck)
					{
						CheckXBytes(address, addressOffset, a_bytes.GetLength());
					}
				}
				i_file.BlockSeek(address, addressOffset);
				i_file.Write(a_bytes._buffer, a_bytes.GetLength());
				if (i_backupFile != null)
				{
					i_backupFile.BlockSeek(address, addressOffset);
					i_backupFile.Write(a_bytes._buffer, a_bytes.GetLength());
				}
			}
			catch (System.Exception e)
			{
				Db4objects.Db4o.Inside.Exceptions4.ThrowRuntimeException(16, e);
			}
		}

		public override void DebugWriteXBytes(int a_address, int a_length)
		{
		}

		public virtual void WriteXBytes(int a_address, int a_length)
		{
			if (!ConfigImpl().IsReadOnly())
			{
				if (a_address > 0 && a_length > 0)
				{
					try
					{
						i_file.BlockSeek(a_address);
						i_file.Write(XBytes(a_address, a_length)._buffer, a_length);
					}
					catch (System.Exception e)
					{
						Sharpen.Runtime.PrintStackTrace(e);
					}
				}
			}
		}
	}
}
