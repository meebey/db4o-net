/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

using System;

namespace Db4objects.Db4o.Linq.Caching
{
	public class NullCachingStrategy<TKey, TValue> : ICachingStrategy<TKey, TValue>
		where TKey : class
		where TValue : class
	{	
		public TValue Produce(TKey key, Func<TKey, TValue> producer)
		{
			return producer(key);
		}
	}
}
