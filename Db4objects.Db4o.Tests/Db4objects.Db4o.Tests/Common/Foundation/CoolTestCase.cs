/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Tests.Common.Foundation
{
	public class CoolTestCase : ITestCase
	{
		public virtual void TestLoopWithTimeoutReturnsWhenBlockIsFalse()
		{
			StopWatch watch = new AutoStopWatch();
			Cool.LoopWithTimeout(500, new _IConditionalBlock_14());
			Assert.IsSmaller(500, watch.Peek());
		}

		private sealed class _IConditionalBlock_14 : IConditionalBlock
		{
			public _IConditionalBlock_14()
			{
			}

			public bool Run()
			{
				return false;
			}
		}

		public virtual void TestLoopWithTimeoutReturnsAfterTimeout()
		{
			StopWatch watch = new AutoStopWatch();
			Cool.LoopWithTimeout(500, new _IConditionalBlock_24());
			watch.Stop();
			Assert.IsGreaterOrEqual(500, watch.Elapsed());
		}

		private sealed class _IConditionalBlock_24 : IConditionalBlock
		{
			public _IConditionalBlock_24()
			{
			}

			public bool Run()
			{
				return true;
			}
		}
	}
}
