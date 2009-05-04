/* Copyright (C) 2009  Versant Inc.  http://www.db4o.com */

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

		public void Delete(string uri)
		{
			store.DeleteFile(uri);
		}

		public void Rename(string oldUri, string newUri)
		{
			throw new System.NotImplementedException();
		}

		#endregion
	}
}

#endif
