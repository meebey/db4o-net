/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.IO;

namespace Db4objects.Db4o.IO
{
	public class StorageDecorator : IStorage
	{
		protected readonly IStorage _storage;

		public StorageDecorator(IStorage storage)
		{
			_storage = storage;
		}

		public virtual bool Exists(string uri)
		{
			return _storage.Exists(uri);
		}

		/// <exception cref="Db4oIOException"></exception>
		public virtual IBin Open(string uri, bool lockFile, long initialLength, bool readOnly
			)
		{
			return Decorate(_storage.Open(uri, lockFile, initialLength, readOnly));
		}

		protected virtual IBin Decorate(IBin bin)
		{
			return bin;
		}
	}
}
