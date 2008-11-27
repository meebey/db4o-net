/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.IO;

namespace Db4objects.Db4o.IO
{
	public class BlockAwareBin : Db4objects.Db4o.IO.BinDecorator
	{
		private const int CopySize = 4096;

		private int _blockSize;

		public BlockAwareBin(IBin bin) : base(bin)
		{
		}

		/// <summary>converts address and address offset to an absolute address</summary>
		protected long RegularAddress(int blockAddress, int blockAddressOffset)
		{
			if (0 == _blockSize)
			{
				throw new InvalidOperationException();
			}
			return (long)blockAddress * _blockSize + blockAddressOffset;
		}

		/// <summary>copies a block within a file in block mode</summary>
		/// <exception cref="Db4oIOException"></exception>
		public virtual void BlockCopy(int oldAddress, int oldAddressOffset, int newAddress
			, int newAddressOffset, int length)
		{
			Copy(RegularAddress(oldAddress, oldAddressOffset), RegularAddress(newAddress, newAddressOffset
				), length);
		}

		/// <summary>outside call to set the block size of this adapter</summary>
		public virtual void BlockSize(int blockSize)
		{
			if (blockSize < 1)
			{
				throw new ArgumentException();
			}
			_blockSize = blockSize;
		}

		/// <summary>copies a block within a file in absolute mode</summary>
		/// <exception cref="Db4oIOException"></exception>
		public virtual void Copy(long oldAddress, long newAddress, int length)
		{
			if (DTrace.enabled)
			{
				DTrace.IoCopy.LogLength(newAddress, length);
			}
			if (length > CopySize)
			{
				byte[] buffer = new byte[CopySize];
				int pos = 0;
				while (pos + CopySize < length)
				{
					Copy(buffer, oldAddress + pos, newAddress + pos);
					pos += CopySize;
				}
				oldAddress += pos;
				newAddress += pos;
				length -= pos;
			}
			Copy(new byte[length], oldAddress, newAddress);
		}

		/// <exception cref="Db4oIOException"></exception>
		private void Copy(byte[] buffer, long oldAddress, long newAddress)
		{
			Read(oldAddress, buffer);
			Write(oldAddress, buffer);
		}

		/// <summary>reads a buffer at the seeked address</summary>
		/// <returns>the number of bytes read and returned</returns>
		/// <exception cref="Db4oIOException"></exception>
		public virtual int BlockRead(int address, int offset, byte[] buffer)
		{
			return BlockRead(address, offset, buffer, buffer.Length);
		}

		/// <summary>implement to read a buffer at the seeked address</summary>
		/// <exception cref="Db4oIOException"></exception>
		public virtual int BlockRead(int address, int offset, byte[] bytes, int length)
		{
			return Read(RegularAddress(address, offset), bytes, length);
		}

		/// <summary>reads a buffer at the seeked address</summary>
		/// <returns>the number of bytes read and returned</returns>
		/// <exception cref="Db4oIOException"></exception>
		public virtual int BlockRead(int address, byte[] buffer)
		{
			return BlockRead(address, 0, buffer, buffer.Length);
		}

		/// <summary>implement to read a buffer at the seeked address</summary>
		/// <exception cref="Db4oIOException"></exception>
		public virtual int BlockRead(int address, byte[] bytes, int length)
		{
			return BlockRead(address, 0, bytes, length);
		}

		/// <summary>reads a buffer at the seeked address</summary>
		/// <returns>the number of bytes read and returned</returns>
		/// <exception cref="Db4oIOException"></exception>
		public virtual int Read(long pos, byte[] buffer)
		{
			return Read(pos, buffer, buffer.Length);
		}

		/// <summary>reads a buffer at the seeked address</summary>
		/// <returns>the number of bytes read and returned</returns>
		/// <exception cref="Db4oIOException"></exception>
		public virtual void BlockWrite(int address, int offset, byte[] buffer)
		{
			BlockWrite(address, offset, buffer, buffer.Length);
		}

		/// <summary>implement to read a buffer at the seeked address</summary>
		/// <exception cref="Db4oIOException"></exception>
		public virtual void BlockWrite(int address, int offset, byte[] bytes, int length)
		{
			Write(RegularAddress(address, offset), bytes, length);
		}

		/// <summary>reads a buffer at the seeked address</summary>
		/// <returns>the number of bytes read and returned</returns>
		/// <exception cref="Db4oIOException"></exception>
		public virtual void BlockWrite(int address, byte[] buffer)
		{
			BlockWrite(address, 0, buffer, buffer.Length);
		}

		/// <summary>implement to read a buffer at the seeked address</summary>
		/// <exception cref="Db4oIOException"></exception>
		public virtual void BlockWrite(int address, byte[] bytes, int length)
		{
			BlockWrite(address, 0, bytes, length);
		}

		/// <summary>writes a buffer to the seeked address</summary>
		/// <exception cref="Db4oIOException"></exception>
		public virtual void Write(long pos, byte[] bytes)
		{
			Write(pos, bytes, bytes.Length);
		}

		/// <summary>returns the block size currently used</summary>
		public virtual int BlockSize()
		{
			return _blockSize;
		}
	}
}
