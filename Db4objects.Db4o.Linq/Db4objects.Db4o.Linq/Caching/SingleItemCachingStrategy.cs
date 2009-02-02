/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections.Generic;

namespace Db4objects.Db4o.Linq.Caching
{
	public class SingleItemCachingStrategy<TKey, TValue> : ICachingStrategy<TKey, TValue>
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

		public TValue Produce(TKey key, Func<TKey, TValue> producer)
		{
			lock (_lock)
			{
				if (_keyComparer.Equals(_lastKey, key))
					return _lastValue;

				_lastValue = producer(key);
				_lastKey = key;
				return _lastValue;
			}
		}
	}
}
