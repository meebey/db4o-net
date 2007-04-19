using System.Collections;
using Db4oUnit;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Tests.Common.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	/// <exclude></exclude>
	public class ServerTimeoutTestCase : ClientServerTestCaseBase
	{
		public static void Main(string[] arguments)
		{
			new ServerTimeoutTestCase().RunClientServer();
		}

		protected override void Configure(IConfiguration config)
		{
			config.ClientServer().TimeoutPingClients(1);
			config.ClientServer().TimeoutClientSocket(1);
			config.ClientServer().TimeoutServerSocket(1);
		}

		public virtual void _test()
		{
			ObjectServerImpl serverImpl = (ObjectServerImpl)ClientServerFixture().Server();
			IEnumerator iter = serverImpl.IterateDispatchers();
			iter.MoveNext();
			IServerMessageDispatcher serverDispatcher = (IServerMessageDispatcher)iter.Current;
			IClientMessageDispatcher clientDispatcher = ((ClientObjectContainer)Db()).MessageDispatcher
				();
			clientDispatcher.Close();
			Cool.SleepIgnoringInterruption(1000);
			Assert.IsFalse(serverDispatcher.IsMessageDispatcherAlive());
		}
	}
}
