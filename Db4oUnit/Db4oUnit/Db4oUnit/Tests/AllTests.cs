/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Tests;

namespace Db4oUnit.Tests
{
	public class AllTests : ITestSuiteBuilder
	{
		public virtual TestSuite Build()
		{
			return new ReflectionTestSuiteBuilder(new Type[] { typeof(FrameworkTestCase), typeof(
				AssertTestCase), typeof(TestLifeCycleTestCase), typeof(TestSuiteTestCase), typeof(
				ReflectionTestSuiteBuilderTestCase), typeof(ReinstantiatePerMethodTest) }).Build
				();
		}

		public static void Main(string[] args)
		{
			new TestRunner(typeof(AllTests)).Run();
		}
	}
}
