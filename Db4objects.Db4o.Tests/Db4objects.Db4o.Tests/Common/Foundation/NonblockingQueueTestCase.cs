using Db4oUnit;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Tests.Common.Foundation;

namespace Db4objects.Db4o.Tests.Common.Foundation
{
	public class NonblockingQueueTestCase : Queue4TestCaseBase
	{
		public virtual void TestIterator()
		{
			IQueue4 queue = new NonblockingQueue();
			string[] data = new string[] { "a", "b", "c", "d" };
			for (int idx = 0; idx < data.Length; idx++)
			{
				AssertIterator(queue, data, idx);
				queue.Add(data[idx]);
				AssertIterator(queue, data, idx + 1);
			}
		}

		public virtual void TestNext()
		{
			IQueue4 queue = new BlockingQueue();
			string[] data = new string[] { "a", "b", "c", "d" };
			queue.Add(data[0]);
			Assert.AreSame(data[0], queue.Next());
			queue.Add(data[1]);
			queue.Add(data[2]);
			Assert.AreSame(data[1], queue.Next());
			Assert.AreSame(data[2], queue.Next());
		}
	}
}
