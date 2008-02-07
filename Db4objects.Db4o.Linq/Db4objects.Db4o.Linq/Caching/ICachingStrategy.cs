/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Linq.Caching
{
	public interface ICachingStrategy<TKey, TValue>
	{
		void Add(TKey key, TValue value);
		TValue Get(TKey key);
	}
}
