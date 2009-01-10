/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class HashtableBase
	{
		private const float Fill = 0.5F;

		public int _tableSize;

		public int _mask;

		public int _maximumSize;

		public int _size;

		public HashtableIntEntry[] _table;

		public HashtableBase(int size)
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

		public HashtableBase() : this(1)
		{
		}

		/// <param name="cloneOnlyCtor"></param>
		protected HashtableBase(IDeepClone cloneOnlyCtor)
		{
		}

		public virtual void Clear()
		{
			_size = 0;
			Arrays4.Fill(_table, null);
		}

		private int NewSize(int size)
		{
			return (int)(size / Fill);
		}

		public virtual int Size()
		{
			return _size;
		}

		protected virtual HashtableIntEntry FindWithSameKey(HashtableIntEntry newEntry)
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

		protected virtual int EntryIndex(HashtableIntEntry entry)
		{
			return entry._key & _mask;
		}

		protected virtual void PutEntry(HashtableIntEntry newEntry)
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

		protected virtual Db4objects.Db4o.Foundation.HashtableIterator HashtableIterator(
			)
		{
			return new Db4objects.Db4o.Foundation.HashtableIterator(_table);
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

		public virtual IEnumerator Keys()
		{
			return Iterators.Map(HashtableIterator(), new _IFunction4_129());
		}

		private sealed class _IFunction4_129 : IFunction4
		{
			public _IFunction4_129()
			{
			}

			public object Apply(object current)
			{
				return ((IEntry4)current).Key();
			}
		}

		public virtual IEnumerable Values()
		{
			return new _IEnumerable_137(this);
		}

		private sealed class _IEnumerable_137 : IEnumerable
		{
			public _IEnumerable_137(HashtableBase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public IEnumerator GetEnumerator()
			{
				return this._enclosing.ValuesIterator();
			}

			private readonly HashtableBase _enclosing;
		}

		/// <summary>Iterates through all the values.</summary>
		/// <remarks>Iterates through all the values.</remarks>
		/// <returns>value iterator</returns>
		public virtual IEnumerator ValuesIterator()
		{
			return Iterators.Map(HashtableIterator(), new _IFunction4_150());
		}

		private sealed class _IFunction4_150 : IFunction4
		{
			public _IFunction4_150()
			{
			}

			public object Apply(object current)
			{
				return ((IEntry4)current).Value();
			}
		}

		public override string ToString()
		{
			return Iterators.Join(HashtableIterator(), "{", "}", ", ");
		}

		protected virtual void RemoveEntry(HashtableIntEntry predecessor, HashtableIntEntry
			 entry)
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

		protected virtual object RemoveObjectEntry(int intKey, object objectKey)
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

		protected virtual void RemoveIntEntry(int key)
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
	}
}
