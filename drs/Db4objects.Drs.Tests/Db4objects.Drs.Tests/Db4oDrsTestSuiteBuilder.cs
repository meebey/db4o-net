/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System.Collections;
using Db4oUnit;
using Db4objects.Db4o.Foundation;
using Db4objects.Drs.Tests;

namespace Db4objects.Drs.Tests
{
	public class Db4oDrsTestSuiteBuilder : ITestSuiteBuilder
	{
		public static void Main(string[] args)
		{
			new ConsoleTestRunner(new Db4oDrsTestSuiteBuilder()).Run();
		}

		public virtual IEnumerator GetEnumerator()
		{
			return Iterators.Concat(Iterators.Concat(new DrsTestSuiteBuilder(new Db4oDrsFixture
				("db4o-a"), new Db4oDrsFixture("db4o-b"), typeof(Db4oDrsTestSuite)).GetEnumerator
				(), new DrsTestSuiteBuilder(new Db4oClientServerDrsFixture("db4o-cs-a", unchecked(
				(int)(0xdb40))), new Db4oClientServerDrsFixture("db4o-cs-b", 4455), typeof(Db4oDrsTestSuite
				)).GetEnumerator()), Iterators.Concat(new DrsTestSuiteBuilder(new Db4oDrsFixture
				("db4o-a"), new Db4oClientServerDrsFixture("db4o-cs-b", 4455), typeof(Db4oDrsTestSuite
				)).GetEnumerator(), new DrsTestSuiteBuilder(new Db4oClientServerDrsFixture("db4o-cs-a"
				, 4455), new Db4oDrsFixture("db4o-b"), typeof(Db4oDrsTestSuite)).GetEnumerator()
				));
		}
	}
}
