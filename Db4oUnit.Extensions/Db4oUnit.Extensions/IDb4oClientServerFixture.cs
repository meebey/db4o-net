/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;

namespace Db4oUnit.Extensions
{
	public interface IDb4oClientServerFixture : IDb4oFixture
	{
		IObjectServer Server();

		int ServerPort();

		IExtObjectContainer OpenNewClient();

		bool EmbeddedClients();
	}
}
