using System;
using System.Collections;
using System.Collections.Generic;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.TA;

namespace Db4objects.Db4o.Collections
{
	public partial class ArrayDictionary4<K, V> : IDictionary<K, V>, IActivatable
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
            Activate(ActivationPurpose.Read);

            int index = IndexOfKey(key);
            if (index != -1)
            {
                throw new ArgumentException(string.Format("Key {0} already exists", key));
            }
            Insert(key, value);
		}

		public bool Remove(K key)
		{
			Activate(ActivationPurpose.Read);
            int index = IndexOfKey(key);

            if (index == -1)
            {
                return false;
            }
            Delete(index);
            return true;
		}

		public bool Contains(KeyValuePair<K, V> pair)
		{
			Activate(ActivationPurpose.Read);
            int index = IndexOfKey(pair.Key);
            if (index == -1)
            {
                return false;
            }

            KeyValuePair<K, V> thisKeyValuePair = new KeyValuePair<K, V>(pair.Key, ValueAt(index));
            return EqualityComparer<KeyValuePair<K, V>>.Default.Equals(thisKeyValuePair, pair);
		}

		public void CopyTo(KeyValuePair<K, V>[] array, int count)
		{
			Activate(ActivationPurpose.Read);
            if (array == null)
            {
                throw new ArgumentNullException();
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if ((count >= array.Length) || (Count > (array.Length - count)))
            {
                throw new ArgumentException();
            }
            for (int i = 0; i < Count; i++)
            {
                KeyValuePair<K, V> keyValuePair = new KeyValuePair<K, V>(KeyAt(i), ValueAt(i));
                array[count + i] = keyValuePair;
            }
		}

		public bool Remove(KeyValuePair<K, V> pair)
		{
            if (!Contains(pair))
            {
                return false;
            }

            int index = IndexOfKey(pair.Key);
            Delete(index);
            return true;
		}

		public bool TryGetValue(K key, out V value)
		{
			Activate(ActivationPurpose.Read);
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
            get
            {
				Activate(ActivationPurpose.Read);
                int index = IndexOfKey(key);
                if (index == -1)
                {
                    throw new KeyNotFoundException();
                }
                return ValueAt(index);
            }
            set
            {
				Activate(ActivationPurpose.Read);
                int index = IndexOfKey(key);
                if (index == -1)
                {
                    Add(key, value);
                }
                else
                {
                    Replace(index, value);
                }
            }
		}

		public ICollection<K> Keys
		{
            get
            {
				Activate(ActivationPurpose.Read);
                K[] keys = new K[_endIndex - _startIndex];
                Array.Copy(_keys, keys, _endIndex);
                return keys;
            }
		}

		public bool ContainsKey(K key)
		{
			return ContainsKeyImpl(key);
		}

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
            KeyValuePair<K, V>[] keyValuePairs = new KeyValuePair<K, V>[Count];
            for (int i = 0; i < Count; i++)
            {
                keyValuePairs[i] = new KeyValuePair<K, V>(KeyAt(i), ValueAt(i));
            }
            return new Enumerator(keyValuePairs);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<K, V>>)this).GetEnumerator();
		}

        private int IndexOfKey(K key)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }
            return Array.IndexOf(_keys, key);
        }

        public struct Enumerator : IEnumerator<KeyValuePair<K, V>>
        {
            private KeyValuePair<K, V>[] _keyValuePairs;

            private int _currentIndex;

            public Enumerator(KeyValuePair<K, V>[] keyValuePairs)
            {
                _keyValuePairs = keyValuePairs;
                _currentIndex = -1;
            }

            public KeyValuePair<K, V> Current
            {
                get
                {
                    return _keyValuePairs[_currentIndex];
                }
            }

            public void Dispose()
            {
                Array.Clear(_keyValuePairs, 0, _keyValuePairs.Length);
                _keyValuePairs = null;
            }

            public bool MoveNext()
            {
                if (_currentIndex < _keyValuePairs.Length - 1)
                {
                    _currentIndex++;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            void IEnumerator.Reset()
            {
                _currentIndex = -1;
            }

            Object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }
        }

        #region Sharpen Helpers

        private static K DefaultKeyValue()
        {
            return default(K);
        }

        private static V DefaultValue()
        {
            return default(V);    
        }
                
        private static K[] AllocateKeyStorage(int length)
        {
            return new K[length];
        }

        private static V[] AllocateValueStorage(int length)
        {
            return new V[length];
        }

        #endregion
    }
}
