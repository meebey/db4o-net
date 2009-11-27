/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.Caching;

namespace Db4objects.Db4o.Internal.Caching
{
	/// <exclude></exclude>
	internal class LRUIntCache : IPurgeableCache4
	{
		private readonly IDictionary _slots;

		private readonly CircularIntBuffer4 _lru;

		private readonly int _maxSize;

		internal LRUIntCache(int size)
		{
			_maxSize = size;
			_slots = new Hashtable(size);
			_lru = new CircularIntBuffer4(size);
		}

		public virtual object Produce(object key, IFunction4 producer, IProcedure4 finalizer
			)
		{
			object value = _slots[((int)key)];
			if (value == null)
			{
				object newValue = producer.Apply(((int)key));
				if (newValue == null)
				{
					return null;
				}
				if (_slots.Count >= _maxSize)
				{
					object discarded = Sharpen.Collections.Remove(_slots, _lru.RemoveLast());
					if (null != finalizer)
					{
						finalizer.Apply(discarded);
					}
				}
				_slots[((int)key)] = newValue;
				_lru.AddFirst((((int)key)));
				return newValue;
			}
			_lru.Remove((((int)key)));
			// O(N) 
			_lru.AddFirst((((int)key)));
			return value;
		}

		public virtual IEnumerator GetEnumerator()
		{
			return _slots.Values.GetEnumerator();
		}

		public virtual object Purge(object key)
		{
			object removed = Sharpen.Collections.Remove(_slots, ((int)key));
			if (removed == null)
			{
				return null;
			}
			_lru.Remove((((int)key)));
			return removed;
		}
	}
}
