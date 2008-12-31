/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.IO;

namespace Db4objects.Db4o.IO
{
	/// <summary>Wrapper baseclass for all classes that wrap Storage.</summary>
	/// <remarks>
	/// Wrapper baseclass for all classes that wrap Storage.
	/// Each class that adds functionality to a Storage must
	/// extend this class.
	/// </remarks>
	/// <seealso cref="Db4objects.Db4o.IO.BinDecorator"></seealso>
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

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		public virtual IBin Open(BinConfiguration config)
		{
			return Decorate(_storage.Open(config));
		}

		protected virtual IBin Decorate(IBin bin)
		{
			return bin;
		}
	}
}
