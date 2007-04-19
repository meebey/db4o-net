using Db4oUnit;
using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Tests.Common.Foundation
{
	/// <exclude></exclude>
	public class IntArrayListTestCase : ITestCase
	{
		public virtual void TestIteratorGoesForwards()
		{
			IntArrayList list = new IntArrayList();
			AssertIterator(new int[] {  }, list.IntIterator());
			list.Add(1);
			AssertIterator(new int[] { 1 }, list.IntIterator());
			list.Add(2);
			AssertIterator(new int[] { 1, 2 }, list.IntIterator());
		}

		private void AssertIterator(int[] expected, IIntIterator4 iterator)
		{
			for (int i = 0; i < expected.Length; ++i)
			{
				Assert.IsTrue(iterator.MoveNext());
				Assert.AreEqual(expected[i], iterator.CurrentInt());
				Assert.AreEqual(expected[i], iterator.Current);
			}
			Assert.IsFalse(iterator.MoveNext());
		}
	}
}
