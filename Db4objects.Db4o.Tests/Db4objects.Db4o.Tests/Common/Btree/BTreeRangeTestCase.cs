namespace Db4objects.Db4o.Tests.Common.Btree
{
	public class BTreeRangeTestCase : Db4objects.Db4o.Tests.Common.Btree.BTreeTestCaseBase
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Btree.BTreeRangeTestCase().RunSolo();
		}

		protected override void Db4oSetupAfterStore()
		{
			base.Db4oSetupAfterStore();
			Add(new int[] { 3, 7, 4, 9 });
		}

		public virtual void TestLastPointer()
		{
			AssertLastPointer(8, 7);
			AssertLastPointer(11, 9);
			AssertLastPointer(4, 3);
		}

		private void AssertLastPointer(int searchValue, int expectedValue)
		{
			Db4objects.Db4o.Inside.Btree.IBTreeRange single = Search(searchValue);
			Db4objects.Db4o.Inside.Btree.IBTreeRange smallerRange = single.Smaller();
			Db4objects.Db4o.Inside.Btree.BTreePointer lastPointer = smallerRange.LastPointer(
				);
			Db4oUnit.Assert.AreEqual(expectedValue, lastPointer.Key());
		}

		public virtual void TestSize()
		{
			AssertSize(4, Range(3, 9));
			AssertSize(3, Range(4, 9));
			AssertSize(3, Range(3, 7));
			AssertSize(4, Range(2, 9));
			AssertSize(4, Range(3, 10));
			Add(new int[] { 5, 6, 8, 10, 2, 1 });
			AssertSize(10, Range(1, 10));
			AssertSize(9, Range(1, 9));
			AssertSize(9, Range(2, 10));
			AssertSize(9, Range(2, 11));
			AssertSize(10, Range(0, 10));
		}

		private void AssertSize(int size, Db4objects.Db4o.Inside.Btree.IBTreeRange range)
		{
			Db4oUnit.Assert.AreEqual(size, range.Size());
		}

		public virtual void TestIntersectSingleSingle()
		{
			AssertIntersection(new int[] { 4, 7 }, Range(3, 7), Range(4, 9));
			AssertIntersection(new int[] {  }, Range(3, 4), Range(7, 9));
			AssertIntersection(new int[] { 3, 4, 7, 9 }, Range(3, 9), Range(3, 9));
			AssertIntersection(new int[] { 3, 4, 7, 9 }, Range(3, 10), Range(3, 9));
			AssertIntersection(new int[] {  }, Range(1, 2), Range(3, 9));
		}

		public virtual void TestIntersectSingleUnion()
		{
			Db4objects.Db4o.Inside.Btree.IBTreeRange union = Range(3, 3).Union(Range(7, 9));
			Db4objects.Db4o.Inside.Btree.IBTreeRange single = Range(4, 7);
			AssertIntersection(new int[] { 7 }, union, single);
			AssertIntersection(new int[] { 3, 7 }, union, Range(3, 7));
		}

		public virtual void TestIntersectUnionUnion()
		{
			Db4objects.Db4o.Inside.Btree.IBTreeRange union1 = Range(3, 3).Union(Range(7, 9));
			Db4objects.Db4o.Inside.Btree.IBTreeRange union2 = Range(3, 3).Union(Range(9, 9));
			AssertIntersection(new int[] { 3, 9 }, union1, union2);
		}

		public virtual void TestUnion()
		{
			AssertUnion(new int[] { 3, 4, 7, 9 }, Range(3, 4), Range(7, 9));
			AssertUnion(new int[] { 3, 4, 7, 9 }, Range(3, 7), Range(4, 9));
			AssertUnion(new int[] { 3, 7, 9 }, Range(3, 3), Range(7, 9));
			AssertUnion(new int[] { 3, 9 }, Range(3, 3), Range(9, 9));
		}

		public virtual void TestIsEmpty()
		{
			Db4oUnit.Assert.IsTrue(Range(0, 0).IsEmpty());
			Db4oUnit.Assert.IsFalse(Range(3, 3).IsEmpty());
			Db4oUnit.Assert.IsFalse(Range(9, 9).IsEmpty());
			Db4oUnit.Assert.IsTrue(Range(10, 10).IsEmpty());
		}

		public virtual void TestUnionWithEmptyDoesNotCreateNewRange()
		{
			Db4objects.Db4o.Inside.Btree.IBTreeRange range = Range(3, 4);
			Db4objects.Db4o.Inside.Btree.IBTreeRange empty = Range(0, 0);
			Db4oUnit.Assert.AreSame(range, range.Union(empty));
			Db4oUnit.Assert.AreSame(range, empty.Union(range));
			Db4objects.Db4o.Inside.Btree.IBTreeRange union = range.Union(Range(8, 9));
			Db4oUnit.Assert.AreSame(union, union.Union(empty));
			Db4oUnit.Assert.AreSame(union, empty.Union(union));
		}

		public virtual void TestUnionsMerge()
		{
			Db4objects.Db4o.Inside.Btree.IBTreeRange range = Range(3, 3).Union(Range(7, 7)).Union
				(Range(4, 4));
			AssertIsRangeSingle(range);
			Db4objects.Db4o.Tests.Common.Btree.BTreeAssert.AssertRange(new int[] { 3, 4, 7 }, 
				range);
		}

		private void AssertIsRangeSingle(Db4objects.Db4o.Inside.Btree.IBTreeRange range)
		{
			Db4oUnit.Assert.IsInstanceOf(typeof(Db4objects.Db4o.Inside.Btree.BTreeRangeSingle)
				, range);
		}

		public virtual void TestUnionsOfUnions()
		{
			Db4objects.Db4o.Inside.Btree.IBTreeRange union1 = Range(3, 4).Union(Range(8, 9));
			Db4objects.Db4o.Tests.Common.Btree.BTreeAssert.AssertRange(new int[] { 3, 4, 9 }, 
				union1);
			Db4objects.Db4o.Tests.Common.Btree.BTreeAssert.AssertRange(new int[] { 3, 4, 7, 9
				 }, union1.Union(Range(7, 7)));
			Db4objects.Db4o.Inside.Btree.IBTreeRange union2 = Range(3, 3).Union(Range(7, 7));
			AssertUnion(new int[] { 3, 4, 7, 9 }, union1, union2);
			AssertIsRangeSingle(union1.Union(union2));
			AssertIsRangeSingle(union2.Union(union1));
			Db4objects.Db4o.Inside.Btree.IBTreeRange union3 = Range(3, 3).Union(Range(9, 9));
			AssertUnion(new int[] { 3, 7, 9 }, union2, union3);
		}

		public virtual void TestExtendToLastOf()
		{
			Db4objects.Db4o.Tests.Common.Btree.BTreeAssert.AssertRange(new int[] { 3, 4, 7 }, 
				Range(3, 7));
			Db4objects.Db4o.Tests.Common.Btree.BTreeAssert.AssertRange(new int[] { 4, 7, 9 }, 
				Range(4, 9));
		}

		public virtual void TestUnionOfOverlappingSingleRangesYieldSingleRange()
		{
			Db4oUnit.Assert.IsInstanceOf(typeof(Db4objects.Db4o.Inside.Btree.BTreeRangeSingle)
				, Range(3, 4).Union(Range(4, 9)));
		}

		private void AssertUnion(int[] expectedKeys, Db4objects.Db4o.Inside.Btree.IBTreeRange
			 range1, Db4objects.Db4o.Inside.Btree.IBTreeRange range2)
		{
			Db4objects.Db4o.Tests.Common.Btree.BTreeAssert.AssertRange(expectedKeys, range1.Union
				(range2));
			Db4objects.Db4o.Tests.Common.Btree.BTreeAssert.AssertRange(expectedKeys, range2.Union
				(range1));
		}

		private void AssertIntersection(int[] expectedKeys, Db4objects.Db4o.Inside.Btree.IBTreeRange
			 range1, Db4objects.Db4o.Inside.Btree.IBTreeRange range2)
		{
			Db4objects.Db4o.Tests.Common.Btree.BTreeAssert.AssertRange(expectedKeys, range1.Intersect
				(range2));
			Db4objects.Db4o.Tests.Common.Btree.BTreeAssert.AssertRange(expectedKeys, range2.Intersect
				(range1));
		}
	}
}
