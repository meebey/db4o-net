/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests
{
	public class Db4oTests : Db4objects.Drs.Tests.DrsTestSuite
	{
		public static int Main(string[] args)
		{
			if (true)
			{
				return new Db4objects.Drs.Tests.Db4oTests().RunDb4oDb4o();
			}
			int failureCount = new Db4objects.Drs.Tests.Db4oTests().Rundb4oCS();
			//new Db4oTests().runCSdb4o();
			failureCount = failureCount + new Db4objects.Drs.Tests.Db4oTests().RunCSCS();
			//new Db4oTests().runDb4oDb4o();
			return failureCount;
		}

		public virtual int RunDb4oDb4o()
		{
			return new Db4oUnit.ConsoleTestRunner(new Db4objects.Drs.Tests.DrsTestSuiteBuilder
				(new Db4objects.Drs.Tests.Db4oDrsFixture("db4o-a"), new Db4objects.Drs.Tests.Db4oDrsFixture
				("db4o-b"), GetType())).Run();
		}

		public virtual int RunCSCS()
		{
			return new Db4oUnit.ConsoleTestRunner(new Db4objects.Drs.Tests.DrsTestSuiteBuilder
				(new Db4objects.Drs.Tests.Db4oClientServerDrsFixture("db4o-cs-a", unchecked((int
				)(0xdb40))), new Db4objects.Drs.Tests.Db4oClientServerDrsFixture("db4o-cs-b", 4455
				), GetType())).Run();
		}

		public virtual int Rundb4oCS()
		{
			return new Db4oUnit.ConsoleTestRunner(new Db4objects.Drs.Tests.DrsTestSuiteBuilder
				(new Db4objects.Drs.Tests.Db4oDrsFixture("db4o-a"), new Db4objects.Drs.Tests.Db4oClientServerDrsFixture
				("db4o-cs-b", 4455), GetType())).Run();
		}

		public virtual void RunCSdb4o()
		{
			new Db4oUnit.ConsoleTestRunner(new Db4objects.Drs.Tests.DrsTestSuiteBuilder(new Db4objects.Drs.Tests.Db4oClientServerDrsFixture
				("db4o-cs-a", 4455), new Db4objects.Drs.Tests.Db4oDrsFixture("db4o-b"), GetType(
				))).Run();
		}

		protected override System.Type[] SpecificTestCases()
		{
			return new System.Type[] { typeof(Db4objects.Drs.Tests.Dotnet.StructTestCase) };
		}

		protected virtual System.Type[] One()
		{
			return new System.Type[] { typeof(Db4objects.Drs.Tests.ByteArrayTest) };
		}
	}
}
