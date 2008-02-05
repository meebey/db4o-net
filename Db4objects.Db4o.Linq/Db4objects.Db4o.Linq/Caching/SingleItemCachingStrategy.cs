/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections.Generic;

namespace Db4objects.Db4o.Linq.Caching
{
	internal class SingleItemCachingStrategy<TKey, TValue> : ICachingStrategy<TKey, TValue>
	{
		private TKey _lastKey;
		private TValue _lastValue;
		private IEqualityComparer<TKey> _keyComparer;
		private object _lock = new object();

		public SingleItemCachingStrategy() : this(EqualityComparer<TKey>.Default)
		{
		}

		public SingleItemCachingStrategy(IEqualityComparer<TKey> keyComparer)
		{
			_keyComparer = keyComparer;
		}

		public void Add(TKey key, TValue value)
		{
			lock (_lock)
			{
				_lastKey = key;
				_lastValue = value;
			}
		}

		public TValue Get(TKey key)
		{
			lock (_lock)
			{
				return _keyComparer.Equals(_lastKey, key) ? _lastValue : default(TValue);
			}
		}
	}
}
