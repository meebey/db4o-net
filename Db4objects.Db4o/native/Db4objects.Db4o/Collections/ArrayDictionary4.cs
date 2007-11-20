using System;
using System.Collections.Generic;

namespace Db4objects.Db4o.Collections
{
	public partial class ArrayDictionary4<K, V> : IDictionary<K, V>
	{
		public int Count
		{
			get { return Size;  }
		}

		public bool IsReadOnly
		{
			get { return false;  }
		}

		public void Add(KeyValuePair<K, V> item)
		{
			Add(item.Key, item.Value);
		}

		public void Add(K key, V value)
		{
			throw new NotImplementedException();
		}

		public bool Remove(K key)
		{
			throw new NotImplementedException();
		}

		public bool Contains(KeyValuePair<K, V> pair)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(KeyValuePair<K, V>[] array, int count)
		{
			throw new NotImplementedException();
		}

		public bool Remove(KeyValuePair<K, V> pair)
		{
			throw new NotImplementedException();
		}

		public bool TryGetValue(K key, out V value)
		{
			int index = IndexOfKey(key);
			if (index == -1)
			{
				value = default(V);
				return false;
			}
			value = ValueAt(index);
			return true;
		}

		public V this[K key]
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public ICollection<K> Keys
		{
			get { throw new NotImplementedException(); }
		}

		public bool ContainsKey(K key)
		{
			return ContainsKeyImpl(key);
		}

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<K, V>>)this).GetEnumerator();
		}

		private int IndexOf(object[] array, object value)
		{
			return System.Array.IndexOf(array, value);
		}
	}
}
