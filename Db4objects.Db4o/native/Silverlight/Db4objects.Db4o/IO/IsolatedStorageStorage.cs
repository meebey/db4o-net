/* Copyright (C) 2009  db4objects Inc.  http://www.db4o.com */

#if SILVERLIGHT

using System.IO.IsolatedStorage;

namespace Db4objects.Db4o.IO
{
	public class IsolatedStorageStorage : IStorage
	{
		static readonly IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();

		#region IStorage Members

		public IBin Open(BinConfiguration config)
		{
			return new IsolatedStorageFileBin(config, store);
		}

		public bool Exists(string uri)
		{
			return store.FileExists(uri);
		}

		#endregion
	}
}

#endif
