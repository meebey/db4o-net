/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests
{
	public class Db4oClientServerDrsFixture : Db4objects.Drs.Tests.Db4oDrsFixture
	{
		private static readonly string Host = "localhost";

		private static readonly string Username = "db4o";

		private static readonly string Password = Username;

		private Db4objects.Db4o.IObjectServer _server;

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
			Db4objects.Db4o.Db4oFactory.Configure().MessageLevel(-1);
			_server = Db4objects.Db4o.Db4oFactory.OpenServer(testFile.GetPath(), _port);
			_server.GrantAccess(Username, Password);
			_db = (Db4objects.Db4o.Ext.IExtObjectContainer)Db4objects.Db4o.Db4oFactory.OpenClient
				(Host, _port, Username, Password);
			_provider = Db4objects.Drs.Db4o.Db4oProviderFactory.NewInstance(_db, _name);
		}
	}
}
