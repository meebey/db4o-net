/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Sharpen;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class InMemoryObjectContainer : LocalObjectContainer
	{
		private bool _closed = false;

		private readonly MemoryFile _memoryFile;

		private int _length = 0;

		/// <exception cref="OldFormatException"></exception>
		protected InMemoryObjectContainer(IConfiguration config, ObjectContainerBase parent
			, MemoryFile memoryFile) : base(config, parent)
		{
			_memoryFile = memoryFile;
			Open();
		}

		public InMemoryObjectContainer(IConfiguration config, MemoryFile memoryFile) : this
			(config, null, memoryFile)
		{
		}

		/// <exception cref="OldFormatException"></exception>
		protected sealed override void OpenImpl()
		{
			byte[] bytes = _memoryFile.GetBytes();
			if (bytes == null || bytes.Length == 0)
			{
				_memoryFile.SetBytes(new byte[_memoryFile.GetInitialSize()]);
				ConfigureNewFile();
				CommitTransaction();
				WriteHeader(false, false);
			}
			else
			{
				_length = bytes.Length;
				ReadThis();
			}
		}

		/// <exception cref="NotSupportedException"></exception>
		public override void Backup(string path)
		{
			throw new NotSupportedException();
		}

		public override void BlockSize(int size)
		{
		}

		// do nothing, blocksize is always 1
		protected override void FreeInternalResources()
		{
		}

		// nothing to do here
		protected override void ShutdownDataStorage()
		{
			if (!_closed)
			{
				byte[] temp = new byte[_length];
				System.Array.Copy(_memoryFile.GetBytes(), 0, temp, 0, _length);
				_memoryFile.SetBytes(temp);
			}
			_closed = true;
			DropReferences();
		}

		protected virtual void DropReferences()
		{
		}

		// do nothing
		public override void Copy(int oldAddress, int oldAddressOffset, int newAddress, int
			 newAddressOffset, int length)
		{
			int fullNewAddress = newAddress + newAddressOffset;
			EnsureMemoryFileSize(fullNewAddress + length);
			byte[] bytes = _memoryFile.GetBytes();
			System.Array.Copy(bytes, oldAddress + oldAddressOffset, bytes, fullNewAddress, length
				);
		}

		public override long FileLength()
		{
			return _length;
		}

		public override string FileName()
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

		public override void ReadBytes(byte[] bytes, int address, int length)
		{
			try
			{
				System.Array.Copy(_memoryFile.GetBytes(), address, bytes, 0, length);
			}
			catch (Exception e)
			{
				Exceptions4.ThrowRuntimeException(13, e);
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

		public override void WriteBytes(ByteArrayBuffer buffer, int address, int addressOffset
			)
		{
			int fullAddress = address + addressOffset;
			int length = buffer.Length();
			EnsureMemoryFileSize(fullAddress + length);
			System.Array.Copy(buffer._buffer, 0, _memoryFile.GetBytes(), fullAddress, length);
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
		}

		public override void OverwriteDeletedBytes(int a_address, int a_length)
		{
		}

		public override void Reserve(int byteCount)
		{
			throw new NotSupportedException();
		}

		public override byte BlockSize()
		{
			return 1;
		}
	}
}
