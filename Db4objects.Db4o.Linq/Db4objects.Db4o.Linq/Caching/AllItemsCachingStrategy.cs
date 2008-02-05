/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections.Generic;

namespace Db4objects.Db4o.Linq.Caching
{
	internal class AllItemsCachingStrategy<TKey, TValue> : ICachingStrategy<TKey, TValue>
	{
		private Dictionary<TKey, TValue> _cache;

		public AllItemsCachingStrategy() : this(EqualityComparer<TKey>.Default)
		{
		}

		public AllItemsCachingStrategy(IEqualityComparer<TKey> keyComparer)
		{
			_cache = new Dictionary<TKey,TValue>(keyComparer);
		}

		public void Add(TKey key, TValue value)
		{
			_cache.Add(key, value);
		}

		public TValue Get(TKey key)
		{
			TValue value;
			if (_cache.TryGetValue(key, out value))
				return value;

			return default(TValue);
		}
	}
}
