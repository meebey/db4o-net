/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Tests.Common.Foundation;
using Sharpen;
using Sharpen.Lang;

namespace Db4objects.Db4o.Tests.Common.Foundation
{
	public class BlockingQueueTestCase : Queue4TestCaseBase
	{
		public virtual void TestIterator()
		{
			IQueue4 queue = new BlockingQueue();
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

		public virtual void TestBlocking()
		{
			IQueue4 queue = new BlockingQueue();
			string[] data = new string[] { "a", "b", "c", "d" };
			queue.Add(data[0]);
			Assert.AreSame(data[0], queue.Next());
			BlockingQueueTestCase.NotifyThread notifyThread = new BlockingQueueTestCase.NotifyThread
				(queue, data[1]);
			notifyThread.Start();
			long start = Runtime.CurrentTimeMillis();
			Assert.AreSame(data[1], queue.Next());
			long end = Runtime.CurrentTimeMillis();
			Assert.IsGreater(500, end - start);
		}

		public virtual void TestStop()
		{
			BlockingQueue queue = new BlockingQueue();
			string[] data = new string[] { "a", "b", "c", "d" };
			queue.Add(data[0]);
			Assert.AreSame(data[0], queue.Next());
			BlockingQueueTestCase.StopThread notifyThread = new BlockingQueueTestCase.StopThread
				(queue);
			notifyThread.Start();
			Assert.Expect(typeof(BlockingQueueStoppedException), new _ICodeBlock_52(queue));
		}

		private sealed class _ICodeBlock_52 : ICodeBlock
		{
			public _ICodeBlock_52(BlockingQueue queue)
			{
				this.queue = queue;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				queue.Next();
			}

			private readonly BlockingQueue queue;
		}

		private class NotifyThread : Thread
		{
			private IQueue4 _queue;

			private object _data;

			internal NotifyThread(IQueue4 queue, object data)
			{
				_queue = queue;
				_data = data;
			}

			public override void Run()
			{
				try
				{
					Thread.Sleep(1000);
				}
				catch (Exception)
				{
				}
				_queue.Add(_data);
			}
		}

		private class StopThread : Thread
		{
			private BlockingQueue _queue;

			internal StopThread(BlockingQueue queue)
			{
				_queue = queue;
			}

			public override void Run()
			{
				try
				{
					Thread.Sleep(1000);
				}
				catch (Exception)
				{
				}
				_queue.Stop();
			}
		}
	}
}
