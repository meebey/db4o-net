using System;
using System.Collections;
using System.Collections.Generic;
using Db4objects.Db4o.TA;
using Db4objects.Db4o.Activation;

namespace Db4objects.Db4o.Collections
{
    public partial class ArrayDictionary4<K, V> : IDictionary<K, V>, IActivatable
    {
        private K[] _keys;
        
        private V[] _values;

        private int _startIndex;

        private int _endIndex;

        [System.NonSerialized]
        private IActivator _activator;

        public virtual void Bind(IActivator activator)
        {
            if (null != _activator)
            {
                throw new InvalidOperationException();
            }
            _activator = activator;
        }

        public virtual void Activate()
        {
            if (_activator == null)
            {
                return;
            }
            _activator.Activate();
        }

        public ArrayDictionary4()
        {
            initializeBackingArray(16);
        }

        public ArrayDictionary4(int initialCapacity)
        {
            initializeBackingArray(initialCapacity);
        }

        private void initializeBackingArray(int length)
        {
            _keys = new K[length];
            _values = new V[length];
        }

        public IEqualityComparer<K> Comparer
        {
            get
            {
                return EqualityComparer<K>.Default;
            }
        }

        //IDictionary

        public V this[K key] 
        {
            get
            {
                Activate();
                int index = KeyIndexOf(key);
                if (index == -1)
                {
                    throw new KeyNotFoundException();
                }
                return _values[index];
            }

            set
            {
                Activate();
                int index = KeyIndexOf(key);
                if (index == -1)
                {
                    Add(key, value);
                }
                else
                {
                    _values[index] = value;
                }
            }
        }

        public ICollection<K> Keys
        {
            get
            {
                Activate();
                K[] keys = new K[_endIndex];
                Array.Copy(_keys, keys, _endIndex);
                return keys;
            }
        }

        public ICollection<V> Values 
        {
            get
            {
                Activate();
                V[] values = new V[_endIndex];
                Array.Copy(_values, values, _endIndex);
                return values;
            }
        }

        private int KeyIndexOf(K key)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }

            return Array.IndexOf(_keys, key);
        }

        public void Add(K key, V value)
        {
            Activate();
            int index = KeyIndexOf(key);
            if (index != -1)
            {
                throw new ArgumentException(string.Format("Key {0} already exists", key));
            }
            EnsureCapacity();
            _keys[_endIndex] = key;
            _values[_endIndex] = value;
            
            _endIndex++;
        }

        private void EnsureCapacity()
        {
            if (_endIndex == _keys.Length)
            {
                K[] newKeys = new K[_keys.Length * 2];
                V[] newValues = new V[_values.Length * 2];
                Array.Copy(_keys, newKeys, _keys.Length);
                Array.Copy(_values, newValues, _values.Length);
                Array.Clear(_keys, 0, _keys.Length);
                Array.Clear(_values, 0, _values.Length);

                _keys = newKeys;
                _values = newValues;
            }
        }

        public bool ContainsKey(K key)
        {
            Activate();
            return (KeyIndexOf(key) != -1);
        }

        public bool Remove(K key)
        {
            Activate();
            int index = KeyIndexOf(key);
            
            if (index == -1)
            {
                return false;
            }
            Delete(index);
            return true;
        }

        private void Delete(int index)
        {
            for (int i = index; i < _endIndex - 1; i++)
            {
                _keys[i] = _keys[i + 1];
                _values[i] = _values[i + 1];
            }
            _endIndex--;
            _keys[_endIndex] = default(K);
            _values[_endIndex] = default(V);
        }

        public bool TryGetValue(K key, out V value)
        {
            Activate();
            int index = KeyIndexOf(key);
            if (index == -1)
            {
                value = default(V);
                return false;
            }
            value = _values[index];
            return true;
        }

        //ICollection

        public int Count 
        {
            get
            {
                Activate();
                return _endIndex - _startIndex;
            }
        }

        bool ICollection<KeyValuePair<K, V>>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        void ICollection<KeyValuePair<K, V>>.Add(KeyValuePair<K, V> keyValuePair)
        {
            Activate();
            Add(keyValuePair.Key, keyValuePair.Value);
        }

        bool ICollection<KeyValuePair<K, V>>.Contains(KeyValuePair<K, V> keyValuePair)
        {
            Activate();
            int index = KeyIndexOf(keyValuePair.Key);
            if (index == -1)
            {
                return false;
            }
            KeyValuePair<K, V> thisKeyValuePair = new KeyValuePair<K, V>(keyValuePair.Key, _values[index]);
            return EqualityComparer<KeyValuePair<K, V>>.Default.Equals(thisKeyValuePair, keyValuePair);
        }

        void ICollection<KeyValuePair<K, V>>.CopyTo(KeyValuePair<K, V>[] array, int index)
        {
            Activate();
            if (array == null)
            {
                throw new ArgumentNullException();
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if ((index >= array.Length) || (Count > (array.Length - index)))
            {
                throw new ArgumentException();
            }
            for (int i = 0; i < Count; i++)
            {
                KeyValuePair<K, V> keyValuePair = new KeyValuePair<K, V>(_keys[i], _values[i]);
                array[index + i] = keyValuePair;
            }
        }

        bool ICollection<KeyValuePair<K, V>>.Remove(KeyValuePair<K, V> keyValuePair)
        {
            Activate();
            int index = KeyIndexOf(keyValuePair.Key);
            if (index == -1)
            {
                return false;
            }
            if (EqualityComparer<V>.Default.Equals(_values[index], keyValuePair.Value))
            {
                Delete(index);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Clear()
        {
            Activate();
            _startIndex = 0;
            _endIndex = 0;
            Array.Clear(_keys, 0, _keys.Length);
            Array.Clear(_values, 0, _values.Length);
        }

        IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator()
        {
            return NewEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return NewEnumerator();
        }

        public Enumerator GetEnumerator()
        {
            return NewEnumerator();
        }

        private Enumerator NewEnumerator()
        {
            KeyValuePair<K, V>[] keyValuePairs = new KeyValuePair<K, V>[Count];
            for (int i = 0; i < Count; i++)
            {
                keyValuePairs[i] = new KeyValuePair<K, V>(_keys[i], _values[i]);
            }
            return new Enumerator(keyValuePairs);
        }

        public struct Enumerator : IEnumerator<KeyValuePair<K, V>>, IEnumerator
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

    }
}
