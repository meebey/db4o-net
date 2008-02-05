/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Linq.Caching
{
	internal class NullCachingStrategy : ICachingStrategy<object, object>
	{
		public void Add(object key, object value)
		{
		}

		public object Get(object key)
		{
			return null;
		}
	}
}
