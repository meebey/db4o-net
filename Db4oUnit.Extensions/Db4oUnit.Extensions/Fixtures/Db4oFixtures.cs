/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4oUnit.Extensions.Fixtures;

namespace Db4oUnit.Extensions.Fixtures
{
	public class Db4oFixtures
	{
		public static Db4oClientServer NewEmbeddedCS()
		{
			return new Db4oClientServer(true, "C/S EMBEDDED");
		}

		public static Db4oClientServer NewNetworkingCS()
		{
			return new Db4oClientServer(false, "C/S");
		}

		public static Db4oSolo NewSolo()
		{
			return new Db4oSolo();
		}

		public static Db4oInMemory NewInMemory()
		{
			return new Db4oInMemory();
		}
	}
}
