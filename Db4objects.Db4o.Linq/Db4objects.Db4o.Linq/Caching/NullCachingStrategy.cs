/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Linq.Caching
{
	public class NullCachingStrategy<TKey, TValue> : ICachingStrategy<TKey, TValue>
		where TKey : class
		where TValue : class
	{
		public void Add(TKey key, TValue value)
		{
		}

		public TValue Get(TKey key)
		{
			return default(TValue);
		}
	}
}
