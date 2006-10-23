namespace Db4objects.Db4o.Tests.Common.Foundation
{
	/// <exclude></exclude>
	public class IntArrayListTestCase : Db4oUnit.ITestCase
	{
		public virtual void TestIteratorGoesBackwards()
		{
			Db4objects.Db4o.Foundation.IntArrayList list = new Db4objects.Db4o.Foundation.IntArrayList
				();
			AssertIterator(new int[] {  }, list.IntIterator());
			list.Add(1);
			AssertIterator(new int[] { 1 }, list.IntIterator());
			list.Add(2);
			AssertIterator(new int[] { 2, 1 }, list.IntIterator());
		}

		private void AssertIterator(int[] expected, Db4objects.Db4o.Foundation.IIntIterator4
			 iterator)
		{
			for (int i = 0; i < expected.Length; ++i)
			{
				Db4oUnit.Assert.IsTrue(iterator.MoveNext());
				Db4oUnit.Assert.AreEqual(expected[i], iterator.CurrentInt());
				Db4oUnit.Assert.AreEqual(expected[i], iterator.Current);
			}
			Db4oUnit.Assert.IsFalse(iterator.MoveNext());
		}
	}
}
