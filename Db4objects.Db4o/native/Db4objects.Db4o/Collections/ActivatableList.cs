/* Copyright (C) 2010  Versant Inc.   http://www.db4o.com */

using System.Collections;
using System.Collections.Generic;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.Internal.Activation;

namespace Db4objects.Db4o.Collections
{
	public class ActivatableList<T> : ActivatableBase, IActivatableList<T>
	{
		public ActivatableList()
		{
		}

		public ActivatableList(IEnumerable<T> source)
		{
			_list = new List<T>(source);
		}

		public ActivatableList(int capacity)
		{
			_list = new List<T>(capacity);
		}

		public IEnumerator<T> GetEnumerator()
		{
			Activate(ActivationPurpose.Read);
			return _list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(T item)
		{
			ActivateForWrite();
			AsList().Add(item);
		}

		public void AddRange(IEnumerable<T> collection)
		{
			ActivateForWrite();
			AsList().AddRange(collection);
		}

		public int BinarySearch(T item)
		{
			ActivateForRead();
			return AsList().BinarySearch(item);
		}

		public void Clear()
		{
			ActivateForWrite();
			AsList().Clear();
		}

		public bool Contains(T item)
		{
			ActivateForRead();
			return AsList().Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			ActivateForRead();
			AsList().CopyTo(array, arrayIndex);
		}

		public override bool Equals(object obj)
		{
			ActivateForRead();
			return AsList().Equals(obj);
		}

		public bool Remove(T item)
		{
			ActivateForWrite();
			return AsList().Remove(item);
		}

		public int Count
		{
			get
			{
				ActivateForRead();
				return AsList().Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				ActivateForRead();
				return AsIList().IsReadOnly;
			}
		}

		public int IndexOf(T item)
		{
			ActivateForRead();
			return AsList().IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			ActivateForWrite();
			AsList().Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			ActivateForWrite();
			AsList().RemoveAt(index);
		}

		public T this[int index]
		{
			get
			{
				ActivateForRead();
				return AsList()[index];
			}

			set
			{
				ActivateForWrite();
				AsList()[index] = value;
			}
		}

		public void Sort()
		{
			ActivateForWrite();
			AsList().Sort();
		}

		private List<T> AsList()
		{
			if (_list == null)
			{
				_list = new List<T>();
			}

			return _list;
		}

		private IList<T> AsIList()
		{
			return AsList();
		}

		private List<T> _list;
	}
}
