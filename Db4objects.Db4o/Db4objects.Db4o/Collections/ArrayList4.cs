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
		public virtual void Activate(ActivationPurpose purpose)
		{
			if (_activator != null)
			{
				_activator.Activate(purpose);
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

		/// <summary>Same behavior as java.util.ArrayList</summary>
		/// <seealso cref="ArrayList"></seealso>
		public ArrayList4() : this(10)
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
			System.Array.Copy(elements, index, elements, index + 1, listSize - index);
			elements[index] = element;
			IncreaseSize(1);
			MarkModified();
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
			System.Array.Copy(elements, index, elements, index + length, Count - index);
			System.Array.Copy(toBeAdded, 0, elements, index, length);
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
			Arrays.Fill(elements, 0, size, DefaultValue());
			SetSize(0);
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
		public virtual void EnsureCapacity(int minCapacity)
		{
			Activate(ActivationPurpose.READ);
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
			System.Array.Copy(elements, toIndex, elements, fromIndex, size - toIndex);
			Arrays.Fill(elements, size - count, size, DefaultValue());
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
				Activate(ActivationPurpose.READ);
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
			System.Array.Copy(elements, 0, temp, 0, Count);
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
