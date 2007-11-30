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
	/// <summary>Transparent activatable ArrayList implementation.</summary>
	/// <remarks>
	/// Transparent activatable ArrayList implementation.
	/// Implements List interface using an array to store elements.
	/// Each ArrayList4 instance has a capacity, which indicates the
	/// size of the internal array. <br /><br />
	/// When instantiated as a result of a query, all the internal members
	/// are NOT activated at all. When internal members are required to
	/// perform an operation, the instance transparently activates all
	/// the members.
	/// </remarks>
	/// <seealso cref="ArrayList">ArrayList</seealso>
	/// <seealso cref="IActivatable">IActivatable</seealso>
	public partial class ArrayList4<E>
	{
		private E[] elements;

		private int capacity;

		private int listSize;

		[System.NonSerialized]
		private IActivator _activator;

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

		/// <summary>Same behaviour as java.util.ArrayList</summary>
		/// <seealso cref="ArrayList"></seealso>
		public ArrayList4()
		{
		}

		/// <summary>Same behaviour as java.util.ArrayList</summary>
		/// <seealso cref="ArrayList"></seealso>
		public ArrayList4(ICollection<E> c)
		{
			E[] data = CollectionToArray(c);
			capacity = data.Length;
			elements = AllocateStorage(capacity);
			listSize = data.Length;
			System.Array.Copy(data, 0, elements, 0, data.Length);
		}

		/// <summary>Same behaviour as java.util.ArrayList</summary>
		/// <seealso cref="ArrayList"></seealso>
		public ArrayList4(int initialCapacity)
		{
			Initialize(initialCapacity);
		}

		private void Initialize(int initialCapacity)
		{
			if (initialCapacity < 0)
			{
				throw new ArgumentException();
			}
			capacity = initialCapacity;
			elements = AllocateStorage(initialCapacity);
			listSize = AdjustSize(initialCapacity);
		}

		/// <summary>
		/// same as java.util.ArrayList but transparently
		/// activates the members as required.
		/// </summary>
		/// <remarks>
		/// same as java.util.ArrayList but transparently
		/// activates the members as required.
		/// </remarks>
		/// <seealso cref="ArrayList"></seealso>
		/// <seealso cref="IActivatable">IActivatable</seealso>
		internal virtual void Add(int index, E element)
		{
			CheckIndex(index, 0, Count);
			EnsureCapacity(Count + 1);
			E[] array = GetElementsLazy();
			System.Array.Copy(array, index, array, index + 1, listSize - index);
			array[index] = element;
			IncreaseSize(1);
			MarkModified();
		}

		private E[] GetElementsLazy()
		{
			if (elements == null)
			{
				Initialize(10);
			}
			return elements;
		}

		internal bool AddAllImpl(int index, E[] toBeAdded)
		{
			CheckIndex(index, 0, Count);
			int length = toBeAdded.Length;
			if (length == 0)
			{
				return false;
			}
			EnsureCapacity(Count + length);
			E[] array = GetElementsLazy();
			System.Array.Copy(array, index, array, index + length, Count - index);
			System.Array.Copy(toBeAdded, 0, array, index, length);
			IncreaseSize(length);
			MarkModified();
			return true;
		}

		/// <summary>
		/// same as java.util.ArrayList but transparently
		/// activates the members as required.
		/// </summary>
		/// <remarks>
		/// same as java.util.ArrayList but transparently
		/// activates the members as required.
		/// </remarks>
		/// <seealso cref="ArrayList"></seealso>
		/// <seealso cref="IActivatable">IActivatable</seealso>
		public virtual void Clear()
		{
			int size = Count;
			if (size > 0)
			{
				Arrays.Fill(elements, 0, size, DefaultValue());
				SetSize(0);
				MarkModified();
			}
		}

		/// <summary>
		/// same as java.util.ArrayList but transparently
		/// activates the members as required.
		/// </summary>
		/// <remarks>
		/// same as java.util.ArrayList but transparently
		/// activates the members as required.
		/// </remarks>
		/// <seealso cref="ArrayList"></seealso>
		/// <seealso cref="IActivatable">IActivatable</seealso>
		public virtual void EnsureCapacity(int minCapacity)
		{
			Activate();
			if (minCapacity <= capacity)
			{
				return;
			}
			Resize(minCapacity);
		}

		/// <summary>
		/// same as java.util.ArrayList but transparently
		/// activates the members as required.
		/// </summary>
		/// <remarks>
		/// same as java.util.ArrayList but transparently
		/// activates the members as required.
		/// </remarks>
		/// <seealso cref="ArrayList"></seealso>
		/// <seealso cref="IActivatable">IActivatable</seealso>
		public virtual E Get(int index)
		{
			CheckIndex(index, 0, Count - 1);
			return elements[index];
		}

		private int IndexOfImpl(E o)
		{
			for (int index = 0; index < Count; ++index)
			{
				E element = Get(index);
				if (o == null ? element == null : o.Equals(element))
				{
					return index;
				}
			}
			return -1;
		}

		private int LastIndexOf(E o)
		{
			for (int index = Count - 1; index >= 0; --index)
			{
				E element = Get(index);
				if (o == null ? element == null : o.Equals(element))
				{
					return index;
				}
			}
			return -1;
		}

		/// <summary>
		/// same as java.util.ArrayList but transparently
		/// activates the members as required.
		/// </summary>
		/// <remarks>
		/// same as java.util.ArrayList but transparently
		/// activates the members as required.
		/// </remarks>
		/// <seealso cref="ArrayList"></seealso>
		/// <seealso cref="IActivatable">IActivatable</seealso>
		internal virtual E RemoveImpl(int index)
		{
			int size = Count;
			E element = Get(index);
			System.Array.Copy(elements, index + 1, elements, index, size - index - 1);
			elements[size - 1] = DefaultValue();
			DecreaseSize(1);
			MarkModified();
			return element;
		}

		private void RemoveRangeImpl(int fromIndex, int count)
		{
			int size = Count;
			int toIndex = fromIndex + count;
			if ((fromIndex < 0 || fromIndex >= size || toIndex > size || toIndex < fromIndex)
				)
			{
				throw new IndexOutOfRangeException();
			}
			if (count == 0)
			{
				return;
			}
			E[] array = GetElementsLazy();
			System.Array.Copy(array, toIndex, array, fromIndex, size - toIndex);
			Arrays.Fill(array, size - count, size, DefaultValue());
			DecreaseSize(count);
			MarkModified();
		}

		/// <summary>
		/// same as java.util.ArrayList but transparently
		/// activates the members as required.
		/// </summary>
		/// <remarks>
		/// same as java.util.ArrayList but transparently
		/// activates the members as required.
		/// </remarks>
		/// <seealso cref="ArrayList"></seealso>
		/// <seealso cref="IActivatable">IActivatable</seealso>
		internal virtual E Set(int index, E element)
		{
			E oldValue = Get(index);
			elements[index] = element;
			return oldValue;
		}

		/// <summary>
		/// same as java.util.ArrayList but transparently
		/// activates the members as required.
		/// </summary>
		/// <remarks>
		/// same as java.util.ArrayList but transparently
		/// activates the members as required.
		/// </remarks>
		/// <seealso cref="ArrayList"></seealso>
		/// <seealso cref="IActivatable">IActivatable</seealso>
		internal virtual int Size
		{
			get
			{
				Activate();
				return listSize;
			}
		}

		/// <summary>
		/// same as java.util.ArrayList but transparently
		/// activates the members as required.
		/// </summary>
		/// <remarks>
		/// same as java.util.ArrayList but transparently
		/// activates the members as required.
		/// </remarks>
		/// <seealso cref="ArrayList"></seealso>
		/// <seealso cref="IActivatable">IActivatable</seealso>
		public virtual void TrimExcess()
		{
			Resize(Count);
		}

		private void Resize(int minCapacity)
		{
			MarkModified();
			E[] temp = AllocateStorage(minCapacity);
			System.Array.Copy(GetElementsLazy(), 0, temp, 0, Count);
			elements = temp;
			capacity = minCapacity;
		}

		internal virtual void SetSize(int count)
		{
			listSize = count;
		}

		internal virtual void IncreaseSize(int count)
		{
			listSize += count;
		}

		internal virtual void DecreaseSize(int count)
		{
			listSize -= count;
		}

		internal virtual void MarkModified()
		{
			++modCount;
		}
	}
}
