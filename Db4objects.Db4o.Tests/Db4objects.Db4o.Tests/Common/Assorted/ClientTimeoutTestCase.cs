namespace Db4objects.Db4o.Tests.Common.Assorted
{
	/// <exclude></exclude>
	public class ClientTimeoutTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase, Db4oUnit.Extensions.Fixtures.IOptOutSolo
	{
		public static void Main(string[] arguments)
		{
			new Db4objects.Db4o.Tests.Common.Assorted.ClientTimeoutTestCase().RunClientServer
				();
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			config.ClientServer().TimeoutPingClients(10);
			config.ClientServer().TimeoutClientSocket(10);
		}

		public virtual void _test()
		{
			Db4oUnit.Extensions.Fixtures.AbstractClientServerDb4oFixture fixture = (Db4oUnit.Extensions.Fixtures.AbstractClientServerDb4oFixture
				)Fixture();
			Db4objects.Db4o.Internal.CS.ObjectServerImpl serverImpl = (Db4objects.Db4o.Internal.CS.ObjectServerImpl
				)fixture.Server();
			System.Collections.IEnumerator iter = serverImpl.IterateDispatchers();
			iter.MoveNext();
			Db4objects.Db4o.Internal.CS.IServerMessageDispatcher serverDispatcher = (Db4objects.Db4o.Internal.CS.IServerMessageDispatcher
				)iter.Current;
			Db4objects.Db4o.Internal.CS.IClientMessageDispatcher clientDispatcher = ((Db4objects.Db4o.Internal.CS.ClientObjectContainer
				)Db()).MessageDispatcher();
			clientDispatcher.Close();
			Db4objects.Db4o.Foundation.Cool.SleepIgnoringInterruption(1000);
			Db4oUnit.Assert.IsFalse(serverDispatcher.IsMessageDispatcherAlive());
		}
	}
}
