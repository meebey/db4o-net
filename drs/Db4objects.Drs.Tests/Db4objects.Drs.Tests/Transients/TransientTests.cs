/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Drs.Tests;
using Db4objects.Drs.Tests.Transients;

namespace Db4objects.Drs.Tests.Transients
{
	public class TransientTests
	{
		public static void Main(string[] args)
		{
			new TransientTests().RunTransientdb4oCS();
		}

		public virtual void RunTransientdb4oCS()
		{
			new ConsoleTestRunner(new DrsTestSuiteBuilder(new TransientFixture("Transient-a")
				, new Db4oClientServerDrsFixture("db4o-cs-b", 1234), GetType())).Run();
		}
	}
}
