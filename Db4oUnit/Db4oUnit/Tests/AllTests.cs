using System;
using Db4oUnit;
using Db4oUnit.Tests;

namespace Db4oUnit.Tests
{
	public class AllTests : ITestSuiteBuilder
	{
		public virtual TestSuite Build()
		{
			return new ReflectionTestSuiteBuilder(new Type[] { typeof(FrameworkTestCase), typeof(AssertTestCase)
				, typeof(TestLifeCycleTestCase), typeof(ReflectionTestSuiteBuilderTestCase), typeof(ReinstantiatePerMethodTest)
				 }).Build();
		}

		public static void Main(string[] args)
		{
			new TestRunner(typeof(AllTests)).Run();
		}
	}
}
