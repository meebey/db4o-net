/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Tests.Common.Foundation
{
	public class NoDuplicatesQueueTestCase : ITestLifeCycle
	{
		private IQueue4 _queue;

		public virtual void Test()
		{
			_queue.Add("A");
			_queue.Add("B");
			_queue.Add("B");
			_queue.Add("A");
			Assert.AreEqual("A", _queue.Next());
			Assert.AreEqual("B", _queue.Next());
			Assert.IsFalse(_queue.HasNext());
		}

		/// <exception cref="Exception"></exception>
		public virtual void SetUp()
		{
			_queue = new NoDuplicatesQueue(new NonblockingQueue());
		}

		/// <exception cref="Exception"></exception>
		public virtual void TearDown()
		{
			_queue = null;
		}
	}
}
