/* Copyright (C) 2010  Versant Inc.   http://www.db4o.com */

namespace Db4objects.Db4o.Tests.CLI2.Collections.Transparent
{
	class CollectionHolder<T>
	{
		public CollectionHolder(T collection)
		{
			_collection = collection;	
		}

		public T Collection
		{
			get { return _collection; }
		}
		
		private readonly T _collection;
	}
}
