/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using System.Collections.Generic;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.TA;
using Sharpen;
using Sharpen.Util;

namespace Db4objects.Db4o.Collections
{
	/// <summary>Transparent activatable Map implementation.</summary>
	/// <remarks>
	/// Transparent activatable Map implementation.
	/// Implements Map interface using two arrays to store keys and values.<br /><br />
	/// When instantiated as a result of a query, all the internal members
	/// are NOT activated at all. When internal members are required to
	/// perform an operation, the instance transparently activates all
	/// the members.
	/// </remarks>
	/// <seealso cref="IDictionary">IDictionary</seealso>
	/// <seealso cref="IActivatable">IActivatable</seealso>
	public partial class ArrayDictionary4<K, V>
	{
		private object[] _keys;

		private object[] _values;

		private int _startIndex;

		private int _endIndex;

		[System.NonSerialized]
		private IActivator _activator;

		public ArrayDictionary4()
		{
		}

		public ArrayDictionary4(int initialCapacity)
		{
			InitializeBackingArray(initialCapacity);
		}

		/// <summary>activate basic implementation.</summary>
		/// <remarks>activate basic implementation.</remarks>
		/// <seealso cref="IActivatable">IActivatable</seealso>
		public virtual void Activate()
		{
			if (_activator != null)
			{
				_activator.Activate();
			}
		}

		/// <summary>bind basic implementation.</summary>
		/// <remarks>bind basic implementation.</remarks>
		/// <seealso cref="IActivatable">IActivatable</seealso>
		public virtual void Bind(IActivator activator)
		{
			if (_activator != null || activator == null)
			{
				throw new InvalidOperationException();
			}
			_activator = activator;
		}

		private object[] GetKeysArray()
		{
			if (_keys == null)
			{
				InitializeBackingArray(16);
			}
			return _keys;
		}

		private object[] GetValuesArray()
		{
			if (_values == null)
			{
				InitializeBackingArray(16);
			}
			return _values;
		}

		/// <summary>
		/// java.util.Map implementation but transparently
		/// activates the members as required.
		/// </summary>
		/// <remarks>
		/// java.util.Map implementation but transparently
		/// activates the members as required.
		/// </remarks>
		/// <seealso cref="IDictionary"></seealso>
		/// <seealso cref="IActivatable">IActivatable</seealso>
		public virtual void Clear()
		{
			Activate();
			_startIndex = 0;
			_endIndex = 0;
			Arrays.Fill(GetKeysArray(), null);
			Arrays.Fill(GetValuesArray(), null);
		}

		private bool ContainsKeyImpl(K key)
		{
			Activate();
			return IndexOfKey(key) != -1;
		}

		private bool ContainsValueImpl(V value)
		{
			Activate();
			return IndexOfValue(value) != -1;
		}

		private int IndexOfValue(V value)
		{
			return IndexOf(GetValuesArray(), value);
		}

		private V ValueAt(int index)
		{
			return (V)GetValuesArray()[index];
		}

		private K KeyAt(int i)
		{
			return (K)GetKeysArray()[i];
		}

		private V Replace(int index, V value)
		{
			V oldValue = ValueAt(index);
			_values[index] = value;
			return oldValue;
		}

		/// <summary>
		/// java.util.Map implementation but transparently
		/// activates the members as required.
		/// </summary>
		/// <remarks>
		/// java.util.Map implementation but transparently
		/// activates the members as required.
		/// </remarks>
		/// <seealso cref="IDictionary"></seealso>
		/// <seealso cref="IActivatable">IActivatable</seealso>
		internal virtual int Size
		{
			get
			{
				Activate();
				return _endIndex - _startIndex;
			}
		}

		/// <summary>
		/// java.util.Map implementation but transparently
		/// activates the members as required.
		/// </summary>
		/// <remarks>
		/// java.util.Map implementation but transparently
		/// activates the members as required.
		/// </remarks>
		/// <seealso cref="IDictionary"></seealso>
		/// <seealso cref="IActivatable">IActivatable</seealso>
		public virtual ICollection<V> Values
		{
			get
			{
				Activate();
				List<V> list = new List<V>();
				for (int i = _startIndex; i < _endIndex; i++)
				{
					list.Add(ValueAt(i));
				}
				return list;
			}
		}

		/// <summary>
		/// java.util.Map implementation but transparently
		/// activates the members as required.
		/// </summary>
		/// <remarks>
		/// java.util.Map implementation but transparently
		/// activates the members as required.
		/// </remarks>
		/// <seealso cref="IDictionary"></seealso>
		/// <seealso cref="IActivatable">IActivatable</seealso>
		public override int GetHashCode()
		{
			int hashCode = 0;
			foreach (KeyValuePair<K, V> entry in this)
			{
				hashCode += entry.GetHashCode();
			}
			return hashCode;
		}

		private void InitializeBackingArray(int length)
		{
			_keys = new object[length];
			_values = new object[length];
		}

		private void Insert(K key, V value)
		{
			EnsureCapacity();
			_keys[_endIndex] = key;
			_values[_endIndex] = value;
			_endIndex++;
		}

		private void EnsureCapacity()
		{
			if (_keys == null)
			{
				InitializeBackingArray(16);
			}
			if (_endIndex == _keys.Length)
			{
				object[] newKeys = new object[_keys.Length * 2];
				object[] newValues = new object[_values.Length * 2];
				System.Array.Copy(_keys, _startIndex, newKeys, 0, _endIndex - _startIndex);
				System.Array.Copy(_values, _startIndex, newValues, 0, _endIndex - _startIndex);
				Arrays.Fill(_keys, null);
				Arrays.Fill(_values, null);
				_keys = newKeys;
				_values = newValues;
			}
		}

		private V Delete(int index)
		{
			V value = ValueAt(index);
			for (int i = index; i < _endIndex - 1; i++)
			{
				_keys[i] = _keys[i + 1];
				_values[i] = _values[i + 1];
			}
			_endIndex--;
			_keys[_endIndex] = null;
			_values[_endIndex] = null;
			return value;
		}
	}
}
