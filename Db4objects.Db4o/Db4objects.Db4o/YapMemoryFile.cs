namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public class YapMemoryFile : Db4objects.Db4o.YapFile
	{
		private bool i_closed = false;

		internal readonly Db4objects.Db4o.Ext.MemoryFile i_memoryFile;

		private int i_length = 0;

		protected YapMemoryFile(Db4objects.Db4o.Config.IConfiguration config, Db4objects.Db4o.YapStream
			 a_parent, Db4objects.Db4o.Ext.MemoryFile memoryFile) : base(config, a_parent)
		{
			this.i_memoryFile = memoryFile;
			try
			{
				Open();
			}
			catch (System.Exception e)
			{
				Db4objects.Db4o.Inside.Exceptions4.ThrowRuntimeException(22, e);
			}
			Initialize3();
		}

		internal YapMemoryFile(Db4objects.Db4o.Config.IConfiguration config, Db4objects.Db4o.Ext.MemoryFile
			 memoryFile) : this(config, null, memoryFile)
		{
		}

		public override void Backup(string path)
		{
			Db4objects.Db4o.Inside.Exceptions4.ThrowRuntimeException(60);
		}

		public override void BlockSize(int size)
		{
		}

		internal virtual void CheckDemoHop()
		{
		}

		protected override bool Close2()
		{
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
			if (i_closed == false)
			{
				byte[] temp = new byte[i_length];
				System.Array.Copy(i_memoryFile.GetBytes(), 0, temp, 0, i_length);
				i_memoryFile.SetBytes(temp);
			}
			i_closed = true;
			return true;
		}

		public override void Copy(int oldAddress, int oldAddressOffset, int newAddress, int
			 newAddressOffset, int length)
		{
			byte[] bytes = MemoryFileBytes(newAddress + newAddressOffset + length);
			System.Array.Copy(bytes, oldAddress + oldAddressOffset, bytes, newAddress + newAddressOffset
				, length);
		}

		internal override void EmergencyClose()
		{
			base.EmergencyClose();
			i_closed = true;
		}

		public override long FileLength()
		{
			return i_length;
		}

		internal override string FileName()
		{
			return "Memory File";
		}

		internal override bool HasShutDownHook()
		{
			return false;
		}

		public sealed override bool NeedsLockFileThread()
		{
			return false;
		}

		private void Open()
		{
			byte[] bytes = i_memoryFile.GetBytes();
			if (bytes == null || bytes.Length == 0)
			{
				i_memoryFile.SetBytes(new byte[i_memoryFile.GetInitialSize()]);
				ConfigureNewFile();
				Write(false);
				WriteHeader(false);
			}
			else
			{
				i_length = bytes.Length;
				ReadThis();
			}
		}

		public override void ReadBytes(byte[] a_bytes, int a_address, int a_length)
		{
			try
			{
				System.Array.Copy(i_memoryFile.GetBytes(), a_address, a_bytes, 0, a_length);
			}
			catch (System.Exception e)
			{
				Db4objects.Db4o.Inside.Exceptions4.ThrowRuntimeException(13, e);
			}
		}

		public override void ReadBytes(byte[] bytes, int address, int addressOffset, int 
			length)
		{
			ReadBytes(bytes, address + addressOffset, length);
		}

		public override void SyncFiles()
		{
		}

		public override bool WriteAccessTime(int address, int offset, long time)
		{
			return true;
		}

		public override void WriteBytes(Db4objects.Db4o.YapReader a_bytes, int address, int
			 addressOffset)
		{
			int fullAddress = address + addressOffset;
			int length = a_bytes.GetLength();
			System.Array.Copy(a_bytes._buffer, 0, MemoryFileBytes(fullAddress + length), fullAddress
				, length);
		}

		private byte[] MemoryFileBytes(int a_lastByte)
		{
			byte[] bytes = i_memoryFile.GetBytes();
			if (a_lastByte > i_length)
			{
				if (a_lastByte > bytes.Length)
				{
					int increase = a_lastByte - bytes.Length;
					if (increase < i_memoryFile.GetIncrementSizeBy())
					{
						increase = i_memoryFile.GetIncrementSizeBy();
					}
					byte[] temp = new byte[bytes.Length + increase];
					System.Array.Copy(bytes, 0, temp, 0, bytes.Length);
					i_memoryFile.SetBytes(temp);
					bytes = temp;
				}
				i_length = a_lastByte;
			}
			return bytes;
		}

		public override void DebugWriteXBytes(int a_address, int a_length)
		{
		}
	}
}
