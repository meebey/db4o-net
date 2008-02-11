/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Ext;

namespace Db4oUnit.Extensions
{
	public class Db4oClientServerTestCase : AbstractDb4oTestCase, IOptOutSolo
	{
		public virtual IDb4oClientServerFixture ClientServerFixture()
		{
			return (IDb4oClientServerFixture)Fixture();
		}

		public virtual IExtObjectContainer OpenNewClient()
		{
			return ClientServerFixture().OpenNewClient();
		}
	}
}
