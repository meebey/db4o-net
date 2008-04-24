/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Drs.Tests;
using Db4objects.Drs.Tests.Dotnet;

namespace Db4objects.Drs.Tests
{
	public partial class Db4oTests : DrsTestSuite
	{
		public static int Main(string[] args)
		{
			//		if (true) return new Db4oTests().runDb4oDb4o();
			int failureCount = new Db4oTests().Rundb4oCS();
			//new Db4oTests().runCSdb4o();
			failureCount = failureCount + new Db4oTests().RunCSCS();
			//new Db4oTests().runDb4oDb4o();
			return failureCount;
		}

		public virtual int RunDb4oDb4o()
		{
			return new ConsoleTestRunner(new DrsTestSuiteBuilder(new Db4oDrsFixture("db4o-a")
				, new Db4oDrsFixture("db4o-b"), GetType())).Run();
		}

		public virtual int RunCSCS()
		{
			return new ConsoleTestRunner(new DrsTestSuiteBuilder(new Db4oClientServerDrsFixture
				("db4o-cs-a", unchecked((int)(0xdb40))), new Db4oClientServerDrsFixture("db4o-cs-b"
				, 4455), GetType())).Run();
		}

		public virtual int Rundb4oCS()
		{
			return new ConsoleTestRunner(new DrsTestSuiteBuilder(new Db4oDrsFixture("db4o-a")
				, new Db4oClientServerDrsFixture("db4o-cs-b", 4455), GetType())).Run();
		}

		public virtual void RunCSdb4o()
		{
			new ConsoleTestRunner(new DrsTestSuiteBuilder(new Db4oClientServerDrsFixture("db4o-cs-a"
				, 4455), new Db4oDrsFixture("db4o-b"), GetType())).Run();
		}

		protected override Type[] SpecificTestCases()
		{
			return Concat(PlatformSpecificTestCases(), new Type[] { typeof(ArrayTestSuite), typeof(
				CustomArrayListTestCase), typeof(DateReplicationTestCase), typeof(StructTestCase
				), typeof(DeepListGraphTestCase), typeof(UntypedFieldTestCase) });
		}
	}
}
