/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Internal.Query.Processor;
using Sharpen;

namespace Db4objects.Db4o.Internal.Query.Processor
{
	internal class Order : IOrderable
	{
		private int i_major;

		private int[] i_minors = new int[8];

		private int minorsSize;

		public virtual int CompareTo(object obj)
		{
			if (obj is Order)
			{
				Order other = (Order)obj;
				int res = i_major - other.i_major;
				if (res != 0)
				{
					return res;
				}
				return CompareMinors(other);
			}
			return -1;
		}

		public virtual void HintOrder(int a_order, bool a_major)
		{
			if (a_major)
			{
				i_major = a_order;
			}
			else
			{
				AppendMinor(a_order);
			}
		}

		public virtual bool HasDuplicates()
		{
			return true;
		}

		public override string ToString()
		{
			string str = "Order " + i_major;
			for (int i = 0; i < minorsSize; i++)
			{
				str = str + " " + i_minors[i];
			}
			return str;
		}

		public virtual void SwapMajorToMinor()
		{
			InsertMinor(i_major);
			i_major = 0;
		}

		private void AppendMinor(int minor)
		{
			EnsureMinorsCapacity();
			i_minors[minorsSize] = minor;
			minorsSize++;
		}

		private void InsertMinor(int minor)
		{
			EnsureMinorsCapacity();
			System.Array.Copy(i_minors, 0, i_minors, 1, minorsSize);
			i_minors[0] = minor;
			minorsSize++;
		}

		private void EnsureMinorsCapacity()
		{
			if (minorsSize == i_minors.Length)
			{
				int[] newMinors = new int[minorsSize * 2];
				System.Array.Copy(i_minors, 0, newMinors, 0, minorsSize);
				i_minors = newMinors;
			}
		}

		private int CompareMinors(Order other)
		{
			if (minorsSize != other.minorsSize)
			{
				throw new Exception("Unexpected exception: this.minorsSize=" + minorsSize + ", other.minorsSize="
					 + other.minorsSize);
			}
			int result = 0;
			for (int i = 0; i < minorsSize; i++)
			{
				if (i_minors[i] == other.i_minors[i])
				{
					continue;
				}
				else
				{
					return (i_minors[i] - other.i_minors[i]);
				}
			}
			return result;
		}
	}
}
