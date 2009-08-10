/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Internal;
using Sharpen;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	[System.ObsoleteAttribute(@"since 7.11: Use  instead")]
	public class InMemoryObjectContainer : LocalObjectContainer
	{
		private bool _closed = false;

		private readonly MemoryFile _memoryFile;

		private int _capacity = 0;

		private int _length = 0;

		/// <exception cref="Db4objects.Db4o.Ext.OldFormatException"></exception>
		public InMemoryObjectContainer(IConfiguration config, MemoryFile memoryFile) : base
			(config)
		{
			_memoryFile = memoryFile;
			Open();
		}

		protected sealed class ConstructionMode
		{
		}

		protected static readonly InMemoryObjectContainer.ConstructionMode DeferredOpenMode
			 = new InMemoryObjectContainer.ConstructionMode();

		protected InMemoryObjectContainer(Config4Impl config, MemoryFile memoryFile, InMemoryObjectContainer.ConstructionMode
			 ignored) : base(config)
		{
			_memoryFile = memoryFile;
		}

		public virtual void DeferredOpen()
		{
			Open();
		}

		/// <exception cref="Db4objects.Db4o.Ext.OldFormatException"></exception>
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
				_length = _capacity = bytes.Length;
				ReadThis();
			}
		}

		/// <exception cref="System.NotSupportedException"></exception>
		public override void Backup(IStorage targetStorage, string path)
		{
			throw new NotSupportedException();
		}

		public override void BlockSize(int size)
		{
		}

		// do nothing, blocksize is always 1
		protected override void CloseSystemTransaction()
		{
		}

		// do nothing
		protected override void FreeInternalResources()
		{
		}

		// nothing to do here
		protected override void ShutdownDataStorage()
		{
			if (!_closed)
			{
				byte[] temp = new byte[_capacity];
				System.Array.Copy(_memoryFile.GetBytes(), 0, temp, 0, _capacity);
				_memoryFile.SetBytes(temp);
			}
			_closed = true;
			DropReferences();
		}

		protected virtual void DropReferences()
		{
		}

		// do nothing
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
			_length = Math.Max(_length, fullAddress + length + 1);
		}

		private void EnsureMemoryFileSize(int last)
		{
			if (last < _capacity)
			{
				return;
			}
			byte[] bytes = _memoryFile.GetBytes();
			if (last < bytes.Length)
			{
				_capacity = last;
				return;
			}
			int increment = _memoryFile.GetIncrementSizeBy();
			while (last > (increment + bytes.Length))
			{
				increment <<= 1;
			}
			byte[] newBytes = new byte[bytes.Length + increment];
			System.Array.Copy(bytes, 0, newBytes, 0, bytes.Length);
			_memoryFile.SetBytes(newBytes);
			_capacity = newBytes.Length;
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

		protected override void FatalStorageShutdown()
		{
			ShutdownDataStorage();
		}
	}
}
