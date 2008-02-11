/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class Hashtable4 : IDeepClone
	{
		private const float Fill = 0.5F;

		public int _tableSize;

		public int _mask;

		public int _maximumSize;

		public int _size;

		public HashtableIntEntry[] _table;

		public Hashtable4(int size)
		{
			// FIELDS ARE PUBLIC SO THEY CAN BE REFLECTED ON IN JDKs <= 1.1
			size = NewSize(size);
			// legacy for .NET conversion
			_tableSize = 1;
			while (_tableSize < size)
			{
				_tableSize = _tableSize << 1;
			}
			_mask = _tableSize - 1;
			_maximumSize = (int)(_tableSize * Fill);
			_table = new HashtableIntEntry[_tableSize];
		}

		public Hashtable4() : this(1)
		{
		}

		/// <param name="cloneOnlyCtor"></param>
		protected Hashtable4(IDeepClone cloneOnlyCtor)
		{
		}

		public virtual int Size()
		{
			return _size;
		}

		public virtual object DeepClone(object obj)
		{
			return DeepCloneInternal(new Db4objects.Db4o.Foundation.Hashtable4((IDeepClone)null
				), obj);
		}

		public virtual void ForEachKeyForIdentity(IVisitor4 visitor, object obj)
		{
			for (int i = 0; i < _table.Length; i++)
			{
				HashtableIntEntry entry = _table[i];
				while (entry != null)
				{
					if (entry._object == obj)
					{
						visitor.Visit(entry.Key());
					}
					entry = entry._next;
				}
			}
		}

		public virtual object Get(byte[] key)
		{
			int intKey = HashtableByteArrayEntry.Hash(key);
			return GetFromObjectEntry(intKey, key);
		}

		public virtual object Get(int key)
		{
			HashtableIntEntry entry = _table[key & _mask];
			while (entry != null)
			{
				if (entry._key == key)
				{
					return entry._object;
				}
				entry = entry._next;
			}
			return null;
		}

		public virtual object Get(object key)
		{
			if (key == null)
			{
				return null;
			}
			return GetFromObjectEntry(key.GetHashCode(), key);
		}

		/// <summary>
		/// Iterates through all the
		/// <see cref="IEntry4">entries</see>
		/// .
		/// </summary>
		/// <returns>
		/// 
		/// <see cref="IEntry4">IEntry4</see>
		/// iterator
		/// </returns>
		public virtual IEnumerator Iterator()
		{
			return new HashtableIterator(_table);
		}

		/// <summary>Iterates through all the keys.</summary>
		/// <remarks>Iterates through all the keys.</remarks>
		/// <returns>key iterator</returns>
		public virtual IEnumerator Keys()
		{
			return Iterators.Map(Iterator(), new _IFunction4_102(this));
		}

		private sealed class _IFunction4_102 : IFunction4
		{
			public _IFunction4_102(Hashtable4 _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Apply(object current)
			{
				return ((IEntry4)current).Key();
			}

			private readonly Hashtable4 _enclosing;
		}

		/// <summary>Iterates through all the values.</summary>
		/// <remarks>Iterates through all the values.</remarks>
		/// <returns>value iterator</returns>
		public virtual IEnumerator Values()
		{
			return Iterators.Map(Iterator(), new _IFunction4_115(this));
		}

		private sealed class _IFunction4_115 : IFunction4
		{
			public _IFunction4_115(Hashtable4 _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Apply(object current)
			{
				return ((IEntry4)current).Value();
			}

			private readonly Hashtable4 _enclosing;
		}

		public virtual bool ContainsKey(object key)
		{
			if (null == key)
			{
				return false;
			}
			return null != GetObjectEntry(key.GetHashCode(), key);
		}

		public virtual bool ContainsAllKeys(IEnumerable collection)
		{
			return ContainsAllKeys(collection.GetEnumerator());
		}

		public virtual bool ContainsAllKeys(IEnumerator iterator)
		{
			while (iterator.MoveNext())
			{
				if (!ContainsKey(iterator.Current))
				{
					return false;
				}
			}
			return true;
		}

		public virtual void Put(byte[] key, object value)
		{
			PutEntry(new HashtableByteArrayEntry(key, value));
		}

		public virtual void Put(int key, object value)
		{
			PutEntry(new HashtableIntEntry(key, value));
		}

		public virtual void Put(object key, object value)
		{
			if (null == key)
			{
				throw new ArgumentNullException();
			}
			PutEntry(new HashtableObjectEntry(key, value));
		}

		public virtual object Remove(byte[] key)
		{
			int intKey = HashtableByteArrayEntry.Hash(key);
			return RemoveObjectEntry(intKey, key);
		}

		public virtual void Remove(int key)
		{
			HashtableIntEntry entry = _table[key & _mask];
			HashtableIntEntry predecessor = null;
			while (entry != null)
			{
				if (entry._key == key)
				{
					RemoveEntry(predecessor, entry);
					return;
				}
				predecessor = entry;
				entry = entry._next;
			}
		}

		public virtual void Remove(object objectKey)
		{
			int intKey = objectKey.GetHashCode();
			RemoveObjectEntry(intKey, objectKey);
		}

		public override string ToString()
		{
			return Iterators.Join(Iterator(), "{", "}", ", ");
		}

		protected virtual Db4objects.Db4o.Foundation.Hashtable4 DeepCloneInternal(Db4objects.Db4o.Foundation.Hashtable4
			 ret, object obj)
		{
			ret._mask = _mask;
			ret._maximumSize = _maximumSize;
			ret._size = _size;
			ret._tableSize = _tableSize;
			ret._table = new HashtableIntEntry[_tableSize];
			for (int i = 0; i < _tableSize; i++)
			{
				if (_table[i] != null)
				{
					ret._table[i] = (HashtableIntEntry)_table[i].DeepClone(obj);
				}
			}
			return ret;
		}

		private int EntryIndex(HashtableIntEntry entry)
		{
			return entry._key & _mask;
		}

		private HashtableIntEntry FindWithSameKey(HashtableIntEntry newEntry)
		{
			HashtableIntEntry existing = _table[EntryIndex(newEntry)];
			while (null != existing)
			{
				if (existing.SameKeyAs(newEntry))
				{
					return existing;
				}
				existing = existing._next;
			}
			return null;
		}

		private object GetFromObjectEntry(int intKey, object objectKey)
		{
			HashtableObjectEntry entry = GetObjectEntry(intKey, objectKey);
			return entry == null ? null : entry._object;
		}

		private HashtableObjectEntry GetObjectEntry(int intKey, object objectKey)
		{
			HashtableObjectEntry entry = (HashtableObjectEntry)_table[intKey & _mask];
			while (entry != null)
			{
				if (entry._key == intKey && entry.HasKey(objectKey))
				{
					return entry;
				}
				entry = (HashtableObjectEntry)entry._next;
			}
			return null;
		}

		private void IncreaseSize()
		{
			_tableSize = _tableSize << 1;
			_maximumSize = _maximumSize << 1;
			_mask = _tableSize - 1;
			HashtableIntEntry[] temp = _table;
			_table = new HashtableIntEntry[_tableSize];
			for (int i = 0; i < temp.Length; i++)
			{
				Reposition(temp[i]);
			}
		}

		private void Insert(HashtableIntEntry newEntry)
		{
			_size++;
			if (_size > _maximumSize)
			{
				IncreaseSize();
			}
			int index = EntryIndex(newEntry);
			newEntry._next = _table[index];
			_table[index] = newEntry;
		}

		private int NewSize(int size)
		{
			return (int)(size / Fill);
		}

		private void PutEntry(HashtableIntEntry newEntry)
		{
			HashtableIntEntry existing = FindWithSameKey(newEntry);
			if (null != existing)
			{
				Replace(existing, newEntry);
			}
			else
			{
				Insert(newEntry);
			}
		}

		private void RemoveEntry(HashtableIntEntry predecessor, HashtableIntEntry entry)
		{
			if (predecessor != null)
			{
				predecessor._next = entry._next;
			}
			else
			{
				_table[EntryIndex(entry)] = entry._next;
			}
			_size--;
		}

		private object RemoveObjectEntry(int intKey, object objectKey)
		{
			HashtableObjectEntry entry = (HashtableObjectEntry)_table[intKey & _mask];
			HashtableObjectEntry predecessor = null;
			while (entry != null)
			{
				if (entry._key == intKey && entry.HasKey(objectKey))
				{
					RemoveEntry(predecessor, entry);
					return entry._object;
				}
				predecessor = entry;
				entry = (HashtableObjectEntry)entry._next;
			}
			return null;
		}

		private void Replace(HashtableIntEntry existing, HashtableIntEntry newEntry)
		{
			newEntry._next = existing._next;
			HashtableIntEntry entry = _table[EntryIndex(existing)];
			if (entry == existing)
			{
				_table[EntryIndex(existing)] = newEntry;
			}
			else
			{
				while (entry._next != existing)
				{
					entry = entry._next;
				}
				entry._next = newEntry;
			}
		}

		private void Reposition(HashtableIntEntry entry)
		{
			if (entry != null)
			{
				Reposition(entry._next);
				entry._next = _table[EntryIndex(entry)];
				_table[EntryIndex(entry)] = entry;
			}
		}
	}
}
