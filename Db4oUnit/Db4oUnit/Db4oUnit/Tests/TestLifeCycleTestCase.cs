/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4oUnit;
using Db4oUnit.Tests;
using Db4objects.Db4o.Foundation;

namespace Db4oUnit.Tests
{
	public class TestLifeCycleTestCase : ITestCase
	{
		public virtual void TestLifeCycle()
		{
			IEnumerator tests = new ReflectionTestSuiteBuilder(typeof(RunsLifeCycle)).GetEnumerator
				();
			ITest test = (ITest)Iterators.Next(tests);
			RunsLifeCycle testSubject = (RunsLifeCycle)ReflectionTestSuiteBuilder.GetTestSubject
				(test);
			FrameworkTestCase.RunTestAndExpect(test, 1);
			Assert.IsTrue(testSubject.TearDownCalled());
		}
	}
}
