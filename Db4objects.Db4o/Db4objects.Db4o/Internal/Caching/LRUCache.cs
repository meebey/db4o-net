/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.Caching;

namespace Db4objects.Db4o.Internal.Caching
{
	internal class LRUCache : ICache4
	{
		private readonly IDictionary _slots;

		private readonly CircularBuffer4 _lru;

		private readonly int _maxSize;

		internal LRUCache(int size)
		{
			_maxSize = size;
			_slots = new Hashtable(size);
			_lru = new CircularBuffer4(size);
		}

		public virtual object Produce(object key, IFunction4 producer, IProcedure4 onDiscard
			)
		{
			object value = _slots[key];
			if (value == null)
			{
				if (_slots.Count >= _maxSize)
				{
					object discarded = Sharpen.Util.Collections.Remove(_slots, _lru.RemoveLast());
					if (null != onDiscard)
					{
						onDiscard.Apply(discarded);
					}
				}
				object newValue = producer.Apply(key);
				_slots[key] = newValue;
				_lru.AddFirst(key);
				return newValue;
			}
			_lru.Remove(key);
			// O(N) 
			_lru.AddFirst(key);
			return value;
		}

		public virtual IEnumerator GetEnumerator()
		{
			return _slots.Values.GetEnumerator();
		}
	}
}
