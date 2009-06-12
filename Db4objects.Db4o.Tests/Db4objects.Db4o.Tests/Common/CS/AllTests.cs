/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

#if !SILVERLIGHT
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
			return new Type[] { typeof(Db4objects.Db4o.Tests.Common.CS.Caching.AllTests), typeof(
				Db4objects.Db4o.Tests.Common.CS.Objectexchange.AllTests), typeof(BatchActivationTestCase
				), typeof(CallConstructorsConfigTestCase), typeof(ClientDisconnectTestCase), typeof(
				ClientTimeOutTestCase), typeof(ClientTransactionHandleTestCase), typeof(ClientTransactionPoolTestCase
				), typeof(CloseServerBeforeClientTestCase), typeof(CsCascadedDeleteReaddChildReferenceTestCase
				), typeof(CsDeleteReaddTestCase), typeof(IsAliveTestCase), typeof(NoTestConstructorsQEStringCmpTestCase
				), typeof(ObjectServerTestCase), typeof(PrefetchConfigurationTestCase), typeof(PrimitiveMessageTestCase
				), typeof(QueryConsistencyTestCase), typeof(SendMessageToClientTestCase), typeof(
				ServerClosedTestCase), typeof(ServerPortUsedTestCase), typeof(ServerRevokeAccessTestCase
				), typeof(ServerTimeoutTestCase), typeof(ServerToClientTestCase), typeof(SetSemaphoreTestCase
				), typeof(SwitchingFilesFromClientTestCase), typeof(SwitchingFilesFromMultipleClientsTestCase
				), typeof(SwitchingToFileWithDifferentClassesTestCase) };
		}
	}
}
#endif // !SILVERLIGHT
