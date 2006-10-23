namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class Algorithms4
	{
		public interface IQuickSortable4
		{
			int Size();

			int Compare(int leftIndex, int rightIndex);

			void Swap(int leftIndex, int rightIndex);
		}

		public static void Qsort(Db4objects.Db4o.Foundation.Algorithms4.IQuickSortable4 sortable
			)
		{
			Qsort(sortable, 0, sortable.Size() - 1);
		}

		private static void Qsort(Db4objects.Db4o.Foundation.Algorithms4.IQuickSortable4 
			sortable, int from, int to)
		{
			if (to - from < 1)
			{
				return;
			}
			int pivot = to;
			int left = from;
			int right = to;
			while (left < right)
			{
				while (left < right && sortable.Compare(pivot, left) < 0)
				{
					left++;
				}
				while (left < right && sortable.Compare(pivot, right) >= 0)
				{
					right--;
				}
				Swap(sortable, left, right);
			}
			Swap(sortable, to, right);
			Qsort(sortable, from, right - 1);
			Qsort(sortable, right + 1, to);
		}

		private static void Swap(Db4objects.Db4o.Foundation.Algorithms4.IQuickSortable4 sortable
			, int left, int right)
		{
			if (left == right)
			{
				return;
			}
			sortable.Swap(left, right);
		}
	}
}
