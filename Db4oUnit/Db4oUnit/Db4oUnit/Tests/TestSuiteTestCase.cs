/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Tests;

namespace Db4oUnit.Tests
{
	public class TestSuiteTestCase : ITestCase
	{
		public virtual void TestTestsAreDisposedAfterExecution()
		{
			ITest test = new RunsGreen();
			TestSuite suite = new TestSuite(test);
			ArrayAssert.AreEqual(new ITest[] { test }, suite.GetTests());
			suite.Run(new TestResult());
			Assert.IsNull(suite.GetTests());
		}
	}
}
