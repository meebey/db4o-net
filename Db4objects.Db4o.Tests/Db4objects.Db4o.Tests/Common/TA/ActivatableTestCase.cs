/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions.Mocking;
using Db4objects.Db4o.Tests.Common.TA;

namespace Db4objects.Db4o.Tests.Common.TA
{
	public class ActivatableTestCase : TransparentActivationTestCaseBase
	{
		public virtual void TestActivatorIsBoundUponStore()
		{
			MockActivatable mock = new MockActivatable();
			Store(mock);
			AssertSingleBindCall(mock);
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestActivatorIsBoundUponRetrieval()
		{
			Store(new MockActivatable());
			Reopen();
			AssertSingleBindCall(RetrieveMock());
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestActivatorIsUnboundUponClose()
		{
			MockActivatable mock = new MockActivatable();
			Store(mock);
			Fixture().Close();
			mock.Recorder().Verify(new MethodCall[] { new MethodCall("bind", MethodCall.IgnoredArgument
				), new MethodCall("bind", new object[] { null }) });
		}

		private void AssertSingleBindCall(MockActivatable mock)
		{
			mock.Recorder().Verify(new MethodCall[] { new MethodCall("bind", MethodCall.IgnoredArgument
				) });
		}

		private MockActivatable RetrieveMock()
		{
			return (MockActivatable)RetrieveOnlyInstance(typeof(MockActivatable));
		}
	}
}
