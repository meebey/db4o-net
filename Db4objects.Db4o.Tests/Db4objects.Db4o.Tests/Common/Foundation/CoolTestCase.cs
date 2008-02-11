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
			Cool.LoopWithTimeout(500, new _IConditionalBlock_14(this));
			Assert.IsSmaller(500, watch.Peek());
		}

		private sealed class _IConditionalBlock_14 : IConditionalBlock
		{
			public _IConditionalBlock_14(CoolTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public bool Run()
			{
				return false;
			}

			private readonly CoolTestCase _enclosing;
		}

		public virtual void TestLoopWithTimeoutReturnsAfterTimeout()
		{
			StopWatch watch = new AutoStopWatch();
			Cool.LoopWithTimeout(500, new _IConditionalBlock_24(this));
			watch.Stop();
			Assert.IsGreaterOrEqual(500, watch.Elapsed());
		}

		private sealed class _IConditionalBlock_24 : IConditionalBlock
		{
			public _IConditionalBlock_24(CoolTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public bool Run()
			{
				return true;
			}

			private readonly CoolTestCase _enclosing;
		}
	}
}
