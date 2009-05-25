/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

#if !SILVERLIGHT
using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Tests.Common.Api;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class IsAliveTestCase : TestWithTempFile
	{
		private static readonly string Username = "db4o";

		private static readonly string Password = "db4o";

		public virtual void TestIsAlive()
		{
			IObjectServer server = OpenServer();
			int port = server.Ext().Port();
			ClientObjectContainer client = OpenClient(port);
			Assert.IsTrue(client.IsAlive());
			client.Close();
			server.Close();
		}

		public virtual void TestIsNotAlive()
		{
			IObjectServer server = OpenServer();
			int port = server.Ext().Port();
			ClientObjectContainer client = OpenClient(port);
			server.Close();
			Assert.IsFalse(client.IsAlive());
			client.Close();
		}

		private IConfiguration Config()
		{
			return Db4oFactory.NewConfiguration();
		}

		private IObjectServer OpenServer()
		{
			IObjectServer server = Db4oFactory.OpenServer(Config(), TempFile(), -1);
			server.GrantAccess(Username, Password);
			return server;
		}

		private ClientObjectContainer OpenClient(int port)
		{
			ClientObjectContainer client = (ClientObjectContainer)Db4oFactory.OpenClient(Config
				(), "localhost", port, Username, Password);
			return client;
		}
	}
}
#endif // !SILVERLIGHT
