/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com

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
namespace Db4objects.Drs.Test
{
	public class Db4oClientServerDrsFixture : Db4objects.Drs.Test.Db4oDrsFixture
	{
		private static readonly string HOST = "localhost";

		private static readonly string USERNAME = "db4o";

		private static readonly string PASSWORD = USERNAME;

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
			_server.GrantAccess(USERNAME, PASSWORD);
			_db = (Db4objects.Db4o.Ext.IExtObjectContainer)Db4objects.Db4o.Db4oFactory.OpenClient
				(HOST, _port, USERNAME, PASSWORD);
			_provider = Db4objects.Drs.Db4o.Db4oProviderFactory.NewInstance(_db, _name);
		}
	}
}
