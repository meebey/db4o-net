namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class InMemoryObjectContainer : Db4objects.Db4o.Internal.LocalObjectContainer
	{
		private bool _closed = false;

		private readonly Db4objects.Db4o.Ext.MemoryFile _memoryFile;

		private int _length = 0;

		protected InMemoryObjectContainer(Db4objects.Db4o.Config.IConfiguration config, Db4objects.Db4o.Internal.ObjectContainerBase
			 parent, Db4objects.Db4o.Ext.MemoryFile memoryFile) : base(config, parent)
		{
			_memoryFile = memoryFile;
			try
			{
				Open();
			}
			catch (System.IO.IOException e)
			{
				throw new Db4objects.Db4o.IO.UncheckedIOException(e);
			}
			Initialize3();
		}

		public InMemoryObjectContainer(Db4objects.Db4o.Config.IConfiguration config, Db4objects.Db4o.Ext.MemoryFile
			 memoryFile) : this(config, null, memoryFile)
		{
		}

		public override void Backup(string path)
		{
			Db4objects.Db4o.Internal.Exceptions4.ThrowRuntimeException(60);
		}

		public override void BlockSize(int size)
		{
		}

		protected override void Close2()
		{
			Write(true);
			base.Close2();
			if (!_closed)
			{
				byte[] temp = new byte[_length];
				System.Array.Copy(_memoryFile.GetBytes(), 0, temp, 0, _length);
				_memoryFile.SetBytes(temp);
			}
			_closed = true;
		}

		public override void Copy(int oldAddress, int oldAddressOffset, int newAddress, int
			 newAddressOffset, int length)
		{
			int fullNewAddress = newAddress + newAddressOffset;
			EnsureMemoryFileSize(fullNewAddress + length);
			byte[] bytes = _memoryFile.GetBytes();
			System.Array.Copy(bytes, oldAddress + oldAddressOffset, bytes, fullNewAddress, length
				);
		}

		internal override void EmergencyClose()
		{
			base.EmergencyClose();
			_closed = true;
		}

		public override long FileLength()
		{
			return _length;
		}

		internal override string FileName()
		{
			return "Memory File";
		}

		protected override bool HasShutDownHook()
		{
			return false;
		}

		public sealed override bool NeedsLockFileThread()
		{
			return false;
		}

		private void Open()
		{
			byte[] bytes = _memoryFile.GetBytes();
			if (bytes == null || bytes.Length == 0)
			{
				_memoryFile.SetBytes(new byte[_memoryFile.GetInitialSize()]);
				ConfigureNewFile();
				Write(false);
				WriteHeader(false, false);
			}
			else
			{
				_length = bytes.Length;
				ReadThis();
			}
		}

		public override void ReadBytes(byte[] bytes, int address, int length)
		{
			try
			{
				System.Array.Copy(_memoryFile.GetBytes(), address, bytes, 0, length);
			}
			catch (System.Exception e)
			{
				Db4objects.Db4o.Internal.Exceptions4.ThrowRuntimeException(13, e);
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

		public override void WriteBytes(Db4objects.Db4o.Internal.Buffer bytes, int address
			, int addressOffset)
		{
			int fullAddress = address + addressOffset;
			int length = bytes.GetLength();
			EnsureMemoryFileSize(fullAddress + length);
			System.Array.Copy(bytes._buffer, 0, _memoryFile.GetBytes(), fullAddress, length);
		}

		private void EnsureMemoryFileSize(int last)
		{
			if (last < _length)
			{
				return;
			}
			byte[] bytes = _memoryFile.GetBytes();
			if (last < bytes.Length)
			{
				_length = last;
				return;
			}
			int increment = _memoryFile.GetIncrementSizeBy();
			while (last > increment)
			{
				increment <<= 1;
			}
			byte[] newBytes = new byte[bytes.Length + increment];
			System.Array.Copy(bytes, 0, newBytes, 0, bytes.Length);
			_memoryFile.SetBytes(newBytes);
			_length = newBytes.Length;
			bytes = null;
		}

		public override void OverwriteDeletedBytes(int a_address, int a_length)
		{
		}
	}
}
