/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com

This file is part of the db4o open source object database.

db4o is free software; you can redistribute it and/or modify it under
the terms of version 2 of the GNU General Public License as published
by the Free Software Foundation and as clarified by db4objects' GPL 
interpretation policy, available at
http://www.db4o.com/about/company/legalpolicies/gplinterpretation/
Alternatively you can write to db4objects, Inc., 1900 S Norfolk Street,
Suite 350, San Mateo, CA 94403, USA.

db4o is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or
FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
for more details.

You should have received a copy of the GNU General Public License along
with this program; if not, write to the Free Software Foundation, Inc.,
59 Temple Place - Suite 330, Boston, MA  02111-1307, USA. */
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
