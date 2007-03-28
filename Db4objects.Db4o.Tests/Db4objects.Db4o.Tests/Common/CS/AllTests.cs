namespace Db4objects.Db4o.Tests.Common.CS
{
	public class AllTests : Db4oUnit.Extensions.Db4oTestSuite
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.CS.AllTests().RunClientServer();
		}

		protected override System.Type[] TestCases()
		{
			return new System.Type[] { typeof(Db4objects.Db4o.Tests.Common.CS.ClientServerPingTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.CS.SendMessageToClientTestCase), typeof(Db4objects.Db4o.Tests.Common.CS.ServerRevokeAccessTestCase)
				 };
		}
	}
}
