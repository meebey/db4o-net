namespace Db4objects.Db4o.Tests.Common.Foundation
{
	public class CompositeIterator4TestCase : Db4oUnit.ITestCase
	{
		public virtual void TestWithEmptyIterators()
		{
			AssertIterator(NewIterator());
		}

		public virtual void TestReset()
		{
			Db4objects.Db4o.Foundation.CompositeIterator4 iterator = NewIterator();
			AssertIterator(iterator);
			iterator.Reset();
			AssertIterator(iterator);
		}

		private void AssertIterator(Db4objects.Db4o.Foundation.CompositeIterator4 iterator
			)
		{
			Db4objects.Db4o.Tests.Common.Foundation.IteratorAssert.AreEqual(Db4objects.Db4o.Tests.Common.Foundation.IntArrays4
				.NewIterator(new int[] { 1, 2, 3, 4, 5, 6 }), iterator);
		}

		private Db4objects.Db4o.Foundation.CompositeIterator4 NewIterator()
		{
			Db4objects.Db4o.Foundation.Collection4 iterators = new Db4objects.Db4o.Foundation.Collection4
				();
			iterators.Add(Db4objects.Db4o.Tests.Common.Foundation.IntArrays4.NewIterator(new 
				int[] { 1, 2, 3 }));
			iterators.Add(Db4objects.Db4o.Tests.Common.Foundation.IntArrays4.NewIterator(new 
				int[] {  }));
			iterators.Add(Db4objects.Db4o.Tests.Common.Foundation.IntArrays4.NewIterator(new 
				int[] { 4 }));
			iterators.Add(Db4objects.Db4o.Tests.Common.Foundation.IntArrays4.NewIterator(new 
				int[] { 5, 6 }));
			Db4objects.Db4o.Foundation.CompositeIterator4 iterator = new Db4objects.Db4o.Foundation.CompositeIterator4
				(iterators.GetEnumerator());
			return iterator;
		}
	}
}
