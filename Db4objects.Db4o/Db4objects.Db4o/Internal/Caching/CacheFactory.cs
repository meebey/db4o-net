/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.Caching;

namespace Db4objects.Db4o.Internal.Caching
{
	/// <exclude></exclude>
	public class CacheFactory
	{
		public static ICache4 New2QCache(int size)
		{
			return new LRU2QCache(size);
		}

		public static ICache4 New2QXCache(int size)
		{
			return new LRU2QXCache(size);
		}

		public static ICache4 NewLRUCache(int size)
		{
			return new LRUCache(size);
		}
	}
}
