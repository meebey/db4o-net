namespace Db4objects.Db4o.Tests.Common.Foundation
{
	public class Algorithms4TestCase : Db4oUnit.ITestCase
	{
		public class QuickSortableIntArray : Db4objects.Db4o.Foundation.IQuickSortable4
		{
			internal int[] ints = new int[] { 3, 5, 2, 1, 4 };

			public virtual int Compare(int leftIndex, int rightIndex)
			{
				return ints[leftIndex] - ints[rightIndex];
			}

			public virtual int Size()
			{
				return 5;
			}

			public virtual void Swap(int leftIndex, int rightIndex)
			{
				int temp = ints[leftIndex];
				ints[leftIndex] = ints[rightIndex];
				ints[rightIndex] = temp;
			}

			public virtual void AssertSorted()
			{
				for (int i = 0; i < ints.Length; i++)
				{
					Db4oUnit.Assert.AreEqual(i + 1, ints[i]);
				}
			}
		}

		public virtual void TestQSort()
		{
			Db4objects.Db4o.Tests.Common.Foundation.Algorithms4TestCase.QuickSortableIntArray
				 sample = new Db4objects.Db4o.Tests.Common.Foundation.Algorithms4TestCase.QuickSortableIntArray
				();
			Db4objects.Db4o.Foundation.Algorithms4.Qsort(sample);
			sample.AssertSorted();
		}
	}
}
