using System;
using System.Collections;
using Db4objects.Db4o.TA.Tests.Collections.Internal;

namespace Db4objects.Db4o.TA.Tests.Collections
{
	public class PagedList : IList, IActivatable
	{
		private PagedBackingStore _store = new PagedBackingStore();

		// TA BEGIN
		[NonSerialized]
		TA.Internal.Activator _activator;
		// TA END

		#region IList Members

		public int Add(object value)
		{
			// TA BEGIN
			activate();
			// TA END
			_store.Add(value);
			return _store.Size() - 1;
		}

		public bool Contains(object value)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public int IndexOf(object value)
		{
			throw new NotImplementedException();
		}

		public void Insert(int index, object value)
		{
			throw new NotImplementedException();
		}

		public void Remove(object value)
		{
			throw new NotImplementedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		public object this[int index]
		{
			get
			{
				// TA BEGIN
				activate();
				// TA END
				return _store.Get(index);
			}
			set { throw new NotImplementedException(); }
		}

		public bool IsReadOnly
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsFixedSize
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region ICollection Members

		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		public int Count
		{
			get
			{
				// TA BEGIN
				activate();
				// TA END
				return _store.Size();
			}
		}

		public object SyncRoot
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsSynchronized
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return new ListEnumerator(this);
		}

		class ListEnumerator : IEnumerator
		{
			private IList _list;
			private int _currentIndex;

			public ListEnumerator(IList list)
			{
				_list = list;
				Reset();
			}

			public bool MoveNext()
			{
				if (_currentIndex + 1 < _list.Count)
				{
					++_currentIndex;
					return true;
				}
				return false;
			}

			public void Reset()
			{
				_currentIndex = -1;
			}

			public object Current
			{
				get { return _list[_currentIndex]; }
			}
		}

		#endregion

		#region IActivatable Members

		// TA BEGIN
		public void Bind(IObjectContainer container)
		{
			if (null != _activator)
			{
				_activator.AssertCompatible(container);
				return;
			}
			_activator = new TA.Internal.Activator(container, this);
		}

		private void activate()
		{
			if (_activator == null) return;
			_activator.Activate();
		}
		// TA END

		#endregion
	}
}
