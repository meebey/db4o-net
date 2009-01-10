/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class AllTests : Db4oTestSuite
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.CS.AllTests().RunAll();
		}

		protected override Type[] TestCases()
		{
			return new Type[] { typeof(CallConstructorsConfigTestCase), typeof(ClientDisconnectTestCase
				), typeof(ClientTimeOutTestCase), typeof(ClientTransactionHandleTestCase), typeof(
				ClientTransactionPoolTestCase), typeof(CloseServerBeforeClientTestCase), typeof(
				DeleteReaddTestCase), typeof(IsAliveTestCase), typeof(NoTestConstructorsQEStringCmpTestCase
				), typeof(ObjectServerTestCase), typeof(PrimitiveMessageTestCase), typeof(QueryConsistencyTestCase
				), typeof(SendMessageToClientTestCase), typeof(ServerClosedTestCase), typeof(ServerPortUsedTestCase
				), typeof(ServerRevokeAccessTestCase), typeof(ServerTimeoutTestCase), typeof(ServerToClientTestCase
				), typeof(SetSemaphoreTestCase), typeof(SwitchingFilesFromClientTestCase), typeof(
				SwitchingFilesFromMultipleClientsTestCase) };
		}
	}
}
