/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.IO;

namespace Db4objects.Db4o.IO
{
	/// <exclude></exclude>
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

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		public virtual IBin Open(BinConfiguration config)
		{
			IoAdapterStorage.IoAdapterBin bin = new IoAdapterStorage.IoAdapterBin(_io.Open(config
				.Uri(), config.LockFile(), config.InitialLength(), config.ReadOnly()));
			((IBlockSize)Environments.My(typeof(IBlockSize))).Register(bin);
			return bin;
		}

		internal class IoAdapterBin : IBin, IListener4
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

			public virtual int SyncRead(long position, byte[] bytes, int bytesToRead)
			{
				return Read(position, bytes, bytesToRead);
			}

			public virtual void Write(long position, byte[] bytes, int bytesToWrite)
			{
				_io.Seek(position);
				_io.Write(bytes, bytesToWrite);
			}

			public virtual void BlockSize(int blockSize)
			{
				_io.BlockSize(blockSize);
			}

			public virtual void OnEvent(object @event)
			{
				BlockSize((((int)@event)));
			}
		}
	}
}
