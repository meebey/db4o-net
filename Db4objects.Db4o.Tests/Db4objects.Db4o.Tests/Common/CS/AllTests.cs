using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class AllTests : Db4oTestSuite
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.CS.AllTests().RunClientServer();
		}

		protected override Type[] TestCases()
		{
			return new Type[] { typeof(ClientServerPingTestCase), typeof(SendMessageToClientTestCase)
				, typeof(ServerRevokeAccessTestCase), typeof(ServerTimeoutTestCase) };
		}
	}
}
