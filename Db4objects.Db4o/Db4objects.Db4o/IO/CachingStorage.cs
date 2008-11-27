/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Internal.Caching;

namespace Db4objects.Db4o.IO
{
	public class CachingStorage : Db4objects.Db4o.IO.StorageDecorator
	{
		private static int DefaultPageCount = 64;

		private static int DefaultPageSize = 1024;

		private int _pageCount;

		private int _pageSize;

		public CachingStorage(IStorage storage) : this(storage, DefaultPageCount, DefaultPageSize
			)
		{
		}

		public CachingStorage(IStorage storage, int pageCount, int pageSize) : base(storage
			)
		{
			_pageCount = pageCount;
			_pageSize = pageSize;
		}

		/// <exception cref="Db4oIOException"></exception>
		public override IBin Open(string uri, bool lockFile, long initialLength, bool readOnly
			)
		{
			IBin storage = base.Open(uri, lockFile, initialLength, readOnly);
			if (readOnly)
			{
				return new ReadOnlyBin(new CachingStorage.NonFlushingCachingBin(storage, NewCache
					(), _pageCount, _pageSize));
			}
			return new CachingBin(storage, NewCache(), _pageCount, _pageSize);
		}

		protected virtual ICache4 NewCache()
		{
			return CacheFactory.New2QCache(_pageCount);
		}

		private sealed class NonFlushingCachingBin : Db4objects.Db4o.IO.CachingBin
		{
			/// <exception cref="Db4oIOException"></exception>
			public NonFlushingCachingBin(IBin bin, ICache4 cache, int pageCount, int pageSize
				) : base(bin, cache, pageCount, pageSize)
			{
			}

			protected override void FlushAllPages()
			{
			}
		}
	}
}
