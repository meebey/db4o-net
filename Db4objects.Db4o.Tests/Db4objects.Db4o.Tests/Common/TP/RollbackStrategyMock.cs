/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit.Mocking;
using Db4objects.Db4o;
using Db4objects.Db4o.TA;

namespace Db4objects.Db4o.Tests.Common.TP
{
	public class RollbackStrategyMock : IRollbackStrategy
	{
		private MethodCallRecorder _recorder = new MethodCallRecorder();

		public virtual void Rollback(IObjectContainer container, object obj)
		{
			_recorder.Record(new MethodCall("rollback", container, obj));
		}

		public virtual void Verify(MethodCall[] expectedCalls)
		{
			_recorder.Verify(expectedCalls);
		}
	}
}
