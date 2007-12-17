/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.Query.Processor;

namespace Db4objects.Db4o.Internal.Query.Processor
{
	internal class Order : IOrderable
	{
		private int i_major;

		private IntArrayList i_minors = new IntArrayList();

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
				return CompareMinors(other.i_minors);
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
			for (int i = 0; i < i_minors.Size(); i++)
			{
				str = str + " " + i_minors.Get(i);
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
			i_minors.Add(minor);
		}

		private void InsertMinor(int minor)
		{
			i_minors.Add(0, minor);
		}

		private int CompareMinors(IntArrayList other)
		{
			if (i_minors.Size() != other.Size())
			{
				throw new Exception("Unexpected exception: this..size()=" + i_minors.Size() + ", other.size()="
					 + other.Size());
			}
			int result = 0;
			for (int i = 0; i < i_minors.Size(); i++)
			{
				if (i_minors.Get(i) == other.Get(i))
				{
					continue;
				}
				else
				{
					return (i_minors.Get(i) - other.Get(i));
				}
			}
			return result;
		}
	}
}
