namespace Db4objects.Db4o.Tests.Common.Foundation
{
	public class TreeKeyIteratorTestCase : Db4oUnit.ITestCase
	{
		public static void Main(string[] args)
		{
			new Db4oUnit.TestRunner(typeof(Db4objects.Db4o.Tests.Common.Foundation.TreeKeyIteratorTestCase)
				).Run();
		}

		private static int[] VALUES = new int[] { 1, 3, 5, 7, 9, 10, 11, 13, 24, 76 };

		public virtual void TestIterate()
		{
			for (int i = 1; i <= VALUES.Length; i++)
			{
				AssertIterateValues(VALUES, i);
			}
		}

		public virtual void TestMoveNextAfterCompletion()
		{
			System.Collections.IEnumerator i = new Db4objects.Db4o.Foundation.TreeKeyIterator
				(CreateTree(VALUES));
			while (i.MoveNext())
			{
			}
			Db4oUnit.Assert.IsFalse(i.MoveNext());
		}

		private void AssertIterateValues(int[] values, int count)
		{
			int[] testValues = new int[count];
			System.Array.Copy(values, 0, testValues, 0, count);
			AssertIterateValues(testValues);
		}

		private void AssertIterateValues(int[] values)
		{
			Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor expectingVisitor = new Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor
				(Db4objects.Db4o.Tests.Common.Foundation.IntArrays4.ToObjectArray(values), true, 
				false);
			System.Collections.IEnumerator i = new Db4objects.Db4o.Foundation.TreeKeyIterator
				(CreateTree(values));
			while (i.MoveNext())
			{
				expectingVisitor.Visit(i.Current);
			}
			expectingVisitor.AssertExpectations();
		}

		private Db4objects.Db4o.Foundation.Tree CreateTree(int[] values)
		{
			Db4objects.Db4o.Foundation.Tree tree = new Db4objects.Db4o.Internal.TreeInt(values
				[0]);
			for (int i = 1; i < values.Length; i++)
			{
				tree = tree.Add(new Db4objects.Db4o.Internal.TreeInt(values[i]));
			}
			return tree;
		}

		public virtual void TestEmpty()
		{
			System.Collections.IEnumerator i = new Db4objects.Db4o.Foundation.TreeKeyIterator
				(null);
			Db4oUnit.Assert.IsFalse(i.MoveNext());
		}
	}
}
