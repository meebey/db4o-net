namespace Db4objects.Db4o.Tests.Common.Foundation
{
	public class Queue4TestCase : Db4oUnit.ITestCase
	{
		public virtual void TestIterator()
		{
			Db4objects.Db4o.Foundation.Queue4 queue = new Db4objects.Db4o.Foundation.Queue4();
			string[] data = { "a", "b", "c", "d" };
			for (int idx = 0; idx < data.Length; idx++)
			{
				AssertIterator(queue, data, idx);
				queue.Add(data[idx]);
				AssertIterator(queue, data, idx + 1);
			}
		}

		private void AssertIterator(Db4objects.Db4o.Foundation.Queue4 queue, string[] data
			, int size)
		{
			System.Collections.IEnumerator iter = queue.Iterator();
			for (int idx = 0; idx < size; idx++)
			{
				Db4oUnit.Assert.IsTrue(iter.MoveNext(), "should be able to move in iteration #" +
					 idx + " of " + size);
				Db4oUnit.Assert.AreEqual(data[idx], iter.Current);
			}
			Db4oUnit.Assert.IsFalse(iter.MoveNext());
		}
	}
}
