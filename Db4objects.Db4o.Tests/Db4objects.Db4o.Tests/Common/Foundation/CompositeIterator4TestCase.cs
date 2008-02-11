/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions.Foundation;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Tests.Common.Foundation;

namespace Db4objects.Db4o.Tests.Common.Foundation
{
	public class CompositeIterator4TestCase : ITestCase
	{
		public virtual void TestWithEmptyIterators()
		{
			AssertIterator(NewIterator());
		}

		public virtual void TestReset()
		{
			CompositeIterator4 iterator = NewIterator();
			AssertIterator(iterator);
			iterator.Reset();
			AssertIterator(iterator);
		}

		private void AssertIterator(CompositeIterator4 iterator)
		{
			Iterator4Assert.AreEqual(IntArrays4.NewIterator(new int[] { 1, 2, 3, 4, 5, 6 }), 
				iterator);
		}

		private CompositeIterator4 NewIterator()
		{
			Collection4 iterators = new Collection4();
			iterators.Add(IntArrays4.NewIterator(new int[] { 1, 2, 3 }));
			iterators.Add(IntArrays4.NewIterator(new int[] {  }));
			iterators.Add(IntArrays4.NewIterator(new int[] { 4 }));
			iterators.Add(IntArrays4.NewIterator(new int[] { 5, 6 }));
			CompositeIterator4 iterator = new CompositeIterator4(iterators.GetEnumerator());
			return iterator;
		}
	}
}
