/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using System.Collections.Generic;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.TA;
using Sharpen;

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
			if (_activator == activator)
			{
				return;
			}
			if (activator != null && _activator != null)
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
			elements = AllocateStorage(data.Length);
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
			elements = AllocateStorage(initialCapacity);
			listSize = 0;
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
			ArrayCopyElements(index, index + 1, listSize - index);
			elements[index] = element;
			IncreaseSize(1);
			MarkModified();
		}

		private void ArrayCopyElements(int sourceIndex, int targetIndex, int length)
		{
			ActivateForWrite();
			System.Array.Copy(elements, sourceIndex, elements, targetIndex, length);
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
			ArrayCopyElements(index, index + length, Count - index);
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
			ActivateForWrite();
			Sharpen.Util.Arrays.Fill(elements, 0, size, DefaultValue());
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
			Activate(ActivationPurpose.Read);
			if (minCapacity <= Capacity())
			{
				return;
			}
			Resize(minCapacity);
		}

		private int Capacity()
		{
			return elements.Length;
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
			E element = this[index];
			ArrayCopyElements(index + 1, index, size - index - 1);
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
			Sharpen.Util.Arrays.Fill(elements, size - count, size, DefaultValue());
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
			E oldValue = this[index];
			ActivateForWrite();
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
				Activate(ActivationPurpose.Read);
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
			ActivateForWrite();
			Resize(Count);
		}

		private void Resize(int minCapacity)
		{
			MarkModified();
			E[] temp = AllocateStorage(minCapacity);
			System.Array.Copy(elements, 0, temp, 0, Count);
			elements = temp;
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

		private void ActivateForWrite()
		{
			Activate(ActivationPurpose.Write);
		}
	}
}
