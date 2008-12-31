/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.IO;

namespace Db4objects.Db4o.IO
{
	/// <summary>base class for IoAdapters that delegate to other IoAdapters (decorator pattern)
	/// 	</summary>
	[System.ObsoleteAttribute(@"use")]
	public abstract class VanillaIoAdapter : IoAdapter
	{
		protected IoAdapter _delegate;

		public VanillaIoAdapter(IoAdapter delegateAdapter)
		{
			_delegate = delegateAdapter;
		}

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		protected VanillaIoAdapter(IoAdapter delegateAdapter, string path, bool lockFile, 
			long initialLength, bool readOnly) : this(delegateAdapter.Open(path, lockFile, initialLength
			, readOnly))
		{
		}

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		public override void Close()
		{
			_delegate.Close();
		}

		public override void Delete(string path)
		{
			_delegate.Delete(path);
		}

		public override bool Exists(string path)
		{
			return _delegate.Exists(path);
		}

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		public override long GetLength()
		{
			return _delegate.GetLength();
		}

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		public override int Read(byte[] bytes, int length)
		{
			return _delegate.Read(bytes, length);
		}

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		public override void Seek(long pos)
		{
			_delegate.Seek(pos);
		}

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		public override void Sync()
		{
			_delegate.Sync();
		}

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		public override void Write(byte[] buffer, int length)
		{
			_delegate.Write(buffer, length);
		}
	}
}
