/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Drs.Db4o;
using Db4objects.Drs.Tests;

namespace Db4objects.Drs.Tests
{
	public class Db4oClientServerDrsFixture : Db4oDrsFixture
	{
		private static readonly string Host = "localhost";

		private static readonly string Username = "db4o";

		private static readonly string Password = Username;

		private IObjectServer _server;

		private int _port;

		public Db4oClientServerDrsFixture(string name, int port) : base(name)
		{
			_port = port;
		}

		public override void Close()
		{
			base.Close();
			_server.Close();
		}

		public override void Open()
		{
			Config().MessageLevel(-1);
			IConfiguration clientConfig = (IConfiguration)((IDeepClone)Config()).DeepClone(Config
				());
			_server = Db4oFactory.OpenServer(Config(), testFile.GetPath(), _port);
			_server.GrantAccess(Username, Password);
			_db = (IExtObjectContainer)Db4oFactory.OpenClient(clientConfig, Host, _port, Username
				, Password);
			_provider = Db4oProviderFactory.NewInstance(_db, _name);
		}
	}
}
