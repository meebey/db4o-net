/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.IO;

namespace Db4objects.Db4o.IO
{
	public class IoAdapterStorage : IStorage
	{
		private readonly IoAdapter _io;

		public IoAdapterStorage(IoAdapter io)
		{
			_io = io;
		}

		public virtual bool Exists(string uri)
		{
			return _io.Exists(uri);
		}

		/// <exception cref="Db4oIOException"></exception>
		public virtual IBin Open(string uri, bool lockFile, long initialLength, bool readOnly
			)
		{
			return new IoAdapterStorage.IoAdapterBin(_io.Open(uri, lockFile, initialLength, readOnly
				));
		}

		internal class IoAdapterBin : IBin
		{
			private readonly IoAdapter _io;

			public IoAdapterBin(IoAdapter io)
			{
				_io = io;
			}

			public virtual void Close()
			{
				_io.Close();
			}

			public virtual long Length()
			{
				return _io.GetLength();
			}

			public virtual int Read(long position, byte[] buffer, int bytesToRead)
			{
				_io.Seek(position);
				return _io.Read(buffer, bytesToRead);
			}

			public virtual void Sync()
			{
				_io.Sync();
			}

			public virtual void Write(long position, byte[] bytes, int bytesToWrite)
			{
				_io.Seek(position);
				_io.Write(bytes, bytesToWrite);
			}
		}
	}
}
