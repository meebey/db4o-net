namespace Db4objects.Db4o.IO
{
	/// <summary>
	/// Base class for database file adapters, both for file and memory
	/// databases.
	/// </summary>
	/// <remarks>
	/// Base class for database file adapters, both for file and memory
	/// databases.
	/// </remarks>
	public abstract class IoAdapter
	{
		private const int COPY_SIZE = 4096;

		private int _blockSize;

		/// <summary>converts address and address offset to an absolute address</summary>
		protected long RegularAddress(int blockAddress, int blockAddressOffset)
		{
			if (0 == _blockSize)
			{
				throw new System.InvalidOperationException();
			}
			return (long)blockAddress * _blockSize + blockAddressOffset;
		}

		/// <summary>copies a block within a file in block mode</summary>
		public virtual void BlockCopy(int oldAddress, int oldAddressOffset, int newAddress
			, int newAddressOffset, int length)
		{
			Copy(RegularAddress(oldAddress, oldAddressOffset), RegularAddress(newAddress, newAddressOffset
				), length);
		}

		/// <summary>sets the read/write pointer in the file using block mode</summary>
		public virtual void BlockSeek(int address)
		{
			BlockSeek(address, 0);
		}

		/// <summary>sets the read/write pointer in the file using block mode</summary>
		public virtual void BlockSeek(int address, int offset)
		{
			Seek(RegularAddress(address, offset));
		}

		/// <summary>outside call to set the block size of this adapter</summary>
		public virtual void BlockSize(int blockSize)
		{
			if (blockSize < 1)
			{
				throw new System.ArgumentException();
			}
			_blockSize = blockSize;
		}

		/// <summary>implement to close the adapter</summary>
		public abstract void Close();

		/// <summary>copies a block within a file in absolute mode</summary>
		public virtual void Copy(long oldAddress, long newAddress, int length)
		{
			if (length > COPY_SIZE)
			{
				byte[] buffer = new byte[COPY_SIZE];
				int pos = 0;
				while (pos + COPY_SIZE < length)
				{
					Copy(buffer, oldAddress + pos, newAddress + pos);
					pos += COPY_SIZE;
				}
				oldAddress += pos;
				newAddress += pos;
				length -= pos;
			}
			Copy(new byte[length], oldAddress, newAddress);
		}

		private void Copy(byte[] buffer, long oldAddress, long newAddress)
		{
			Seek(oldAddress);
			Read(buffer);
			Seek(newAddress);
			Write(buffer);
		}

		/// <summary>deletes the given path from whatever 'file system' is addressed</summary>
		public abstract void Delete(string path);

		/// <summary>checks whether a file exists</summary>
		public abstract bool Exists(string path);

		/// <summary>implement to return the absolute length of the file</summary>
		public abstract long GetLength();

		/// <summary>implement to open the file</summary>
		public abstract Db4objects.Db4o.IO.IoAdapter Open(string path, bool lockFile, long
			 initialLength);

		/// <summary>reads a buffer at the seeked address</summary>
		public virtual int Read(byte[] buffer)
		{
			return Read(buffer, buffer.Length);
		}

		/// <summary>implement to read a buffer at the seeked address</summary>
		public abstract int Read(byte[] bytes, int length);

		/// <summary>implement to set the read/write pointer in the file, absolute mode</summary>
		public abstract void Seek(long pos);

		/// <summary>implement to flush the file contents to storage</summary>
		public abstract void Sync();

		/// <summary>writes a buffer to the seeked address</summary>
		public virtual void Write(byte[] bytes)
		{
			Write(bytes, bytes.Length);
		}

		/// <summary>implement to write a buffer at the seeked address</summary>
		public abstract void Write(byte[] buffer, int length);

		/// <summary>returns the block size currently used</summary>
		public virtual int BlockSize()
		{
			return _blockSize;
		}
	}
}
