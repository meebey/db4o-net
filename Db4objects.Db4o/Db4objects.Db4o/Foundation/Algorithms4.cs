/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class Algorithms4
	{
		private class Range
		{
			internal int _from;

			internal int _to;

			public Range(int from, int to)
			{
				_from = from;
				_to = to;
			}
		}

		public static void Qsort(IQuickSortable4 sortable)
		{
			Stack4 stack = new Stack4();
			AddRange(stack, 0, sortable.Size() - 1);
			Qsort(sortable, stack);
		}

		private static void Qsort(IQuickSortable4 sortable, Stack4 stack)
		{
			while (!stack.IsEmpty())
			{
				Algorithms4.Range range = (Algorithms4.Range)stack.Peek();
				stack.Pop();
				int from = range._from;
				int to = range._to;
				int pivot = to;
				int left = from;
				int right = to;
				while (left < right)
				{
					while (left < right && sortable.Compare(left, pivot) < 0)
					{
						left++;
					}
					while (left < right && sortable.Compare(right, pivot) >= 0)
					{
						right--;
					}
					Swap(sortable, left, right);
				}
				Swap(sortable, to, right);
				AddRange(stack, from, right - 1);
				AddRange(stack, right + 1, to);
			}
		}

		private static void AddRange(Stack4 stack, int from, int to)
		{
			if (to - from < 1)
			{
				return;
			}
			stack.Push(new Algorithms4.Range(from, to));
		}

		private static void Swap(IQuickSortable4 sortable, int left, int right)
		{
			if (left == right)
			{
				return;
			}
			sortable.Swap(left, right);
		}
	}
}
