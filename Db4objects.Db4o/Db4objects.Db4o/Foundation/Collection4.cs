namespace Db4objects.Db4o.Foundation
{
	/// <summary>Fast linked list for all usecases.</summary>
	/// <remarks>Fast linked list for all usecases.</remarks>
	/// <exclude></exclude>
	public class Collection4 : System.Collections.IEnumerable, Db4objects.Db4o.Foundation.IDeepClone
		, Db4objects.Db4o.Types.IUnversioned
	{
		/// <summary>first element of the linked list</summary>
		public Db4objects.Db4o.Foundation.List4 _first;

		public Db4objects.Db4o.Foundation.List4 _last;

		/// <summary>number of elements collected</summary>
		public int _size;

		public int _version;

		private static readonly object NOT_FOUND = new object();

		public Collection4()
		{
		}

		public Collection4(System.Collections.IEnumerable other)
		{
			AddAll(other);
		}

		public Collection4(System.Collections.IEnumerator iterator)
		{
			AddAll(iterator);
		}

		public virtual object SingleElement()
		{
			if (Size() != 1)
			{
				throw new System.InvalidOperationException();
			}
			return _first._element;
		}

		/// <summary>Adds an element to the end of this collection.</summary>
		/// <remarks>Adds an element to the end of this collection.</remarks>
		/// <param name="element"></param>
		public void Add(object element)
		{
			DoAdd(element);
			Changed();
		}

		public void Prepend(object element)
		{
			DoPrepend(element);
			Changed();
		}

		private void DoPrepend(object element)
		{
			if (_first == null)
			{
				DoAdd(element);
			}
			else
			{
				_first = new Db4objects.Db4o.Foundation.List4(_first, element);
				_size++;
			}
		}

		private void DoAdd(object element)
		{
			if (_last == null)
			{
				_first = new Db4objects.Db4o.Foundation.List4(element);
				_last = _first;
			}
			else
			{
				_last._next = new Db4objects.Db4o.Foundation.List4(element);
				_last = _last._next;
			}
			_size++;
		}

		public void AddAll(object[] elements)
		{
			AssertNotNull(elements);
			for (int i = 0; i < elements.Length; i++)
			{
				Add(elements[i]);
			}
		}

		public void AddAll(System.Collections.IEnumerable other)
		{
			AssertNotNull(other);
			AddAll(other.GetEnumerator());
		}

		public void AddAll(System.Collections.IEnumerator iterator)
		{
			AssertNotNull(iterator);
			while (iterator.MoveNext())
			{
				Add(iterator.Current);
			}
		}

		public void Clear()
		{
			_first = null;
			_last = null;
			_size = 0;
			Changed();
		}

		public bool Contains(object element)
		{
			return GetInternal(element) != NOT_FOUND;
		}

		public virtual bool ContainsAll(System.Collections.IEnumerator iter)
		{
			AssertNotNull(iter);
			while (iter.MoveNext())
			{
				if (!Contains(iter.Current))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>tests if the object is in the Collection.</summary>
		/// <remarks>tests if the object is in the Collection. == comparison.</remarks>
		public bool ContainsByIdentity(object element)
		{
			System.Collections.IEnumerator i = InternalIterator();
			while (i.MoveNext())
			{
				object current = i.Current;
				if (current == element)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// returns the first object found in the Collections that equals() the
		/// passed object
		/// </summary>
		public object Get(object element)
		{
			object obj = GetInternal(element);
			if (obj == NOT_FOUND)
			{
				return null;
			}
			return obj;
		}

		private object GetInternal(object element)
		{
			if (element == null)
			{
				return ContainsNull() ? null : NOT_FOUND;
			}
			System.Collections.IEnumerator i = InternalIterator();
			while (i.MoveNext())
			{
				object current = i.Current;
				if (element.Equals(current))
				{
					return current;
				}
			}
			return NOT_FOUND;
		}

		private bool ContainsNull()
		{
			return ContainsByIdentity(null);
		}

		public virtual object DeepClone(object newParent)
		{
			Db4objects.Db4o.Foundation.Collection4 col = new Db4objects.Db4o.Foundation.Collection4
				();
			object element = null;
			System.Collections.IEnumerator i = InternalIterator();
			while (i.MoveNext())
			{
				element = i.Current;
				if (element is Db4objects.Db4o.Foundation.IDeepClone)
				{
					col.Add(((Db4objects.Db4o.Foundation.IDeepClone)element).DeepClone(newParent));
				}
				else
				{
					col.Add(element);
				}
			}
			return col;
		}

		/// <summary>makes sure the passed object is in the Collection.</summary>
		/// <remarks>makes sure the passed object is in the Collection. equals() comparison.</remarks>
		public object Ensure(object element)
		{
			object existing = GetInternal(element);
			if (existing == NOT_FOUND)
			{
				Add(element);
				return element;
			}
			return existing;
		}

		/// <summary>
		/// Iterates through the collection in reversed insertion order which happens
		/// to be the fastest.
		/// </summary>
		/// <remarks>
		/// Iterates through the collection in reversed insertion order which happens
		/// to be the fastest.
		/// </remarks>
		/// <returns></returns>
		public System.Collections.IEnumerator GetEnumerator()
		{
			return _first == null ? Db4objects.Db4o.Foundation.Iterator4Impl.EMPTY : new Db4objects.Db4o.Foundation.Collection4Iterator
				(this, _first);
		}

		/// <summary>
		/// removes an object from the Collection equals() comparison returns the
		/// removed object or null, if none found
		/// </summary>
		public virtual object Remove(object a_object)
		{
			Db4objects.Db4o.Foundation.List4 previous = null;
			Db4objects.Db4o.Foundation.List4 current = _first;
			while (current != null)
			{
				if (current.Holds(a_object))
				{
					_size--;
					AdjustOnRemoval(previous, current);
					Changed();
					return current._element;
				}
				previous = current;
				current = current._next;
			}
			return null;
		}

		private void AdjustOnRemoval(Db4objects.Db4o.Foundation.List4 previous, Db4objects.Db4o.Foundation.List4
			 removed)
		{
			if (removed == _first)
			{
				_first = removed._next;
			}
			else
			{
				previous._next = removed._next;
			}
			if (removed == _last)
			{
				_last = previous;
			}
		}

		public int Size()
		{
			return _size;
		}

		public bool IsEmpty()
		{
			return _size == 0;
		}

		/// <summary>This is a non reflection implementation for more speed.</summary>
		/// <remarks>
		/// This is a non reflection implementation for more speed. In contrast to
		/// the JDK behaviour, the passed array has to be initialized to the right
		/// length.
		/// </remarks>
		public object[] ToArray(object[] a_array)
		{
			int j = 0;
			System.Collections.IEnumerator i = InternalIterator();
			while (i.MoveNext())
			{
				a_array[j++] = i.Current;
			}
			return a_array;
		}

		public object[] ToArray()
		{
			object[] array = new object[_size];
			ToArray(array);
			return array;
		}

		public override string ToString()
		{
			if (_size == 0)
			{
				return "[]";
			}
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("[");
			System.Collections.IEnumerator i = InternalIterator();
			i.MoveNext();
			sb.Append(i.Current);
			while (i.MoveNext())
			{
				sb.Append(", ");
				sb.Append(i.Current);
			}
			sb.Append("]");
			return sb.ToString();
		}

		private void Changed()
		{
			++_version;
		}

		internal virtual int Version()
		{
			return _version;
		}

		private void AssertNotNull(object element)
		{
			if (element == null)
			{
				throw new System.ArgumentNullException();
			}
		}

		/// <summary>
		/// Leaner iterator for faster iteration (but unprotected against
		/// concurrent modifications).
		/// </summary>
		/// <remarks>
		/// Leaner iterator for faster iteration (but unprotected against
		/// concurrent modifications).
		/// </remarks>
		private System.Collections.IEnumerator InternalIterator()
		{
			return new Db4objects.Db4o.Foundation.Iterator4Impl(_first);
		}
	}
}
