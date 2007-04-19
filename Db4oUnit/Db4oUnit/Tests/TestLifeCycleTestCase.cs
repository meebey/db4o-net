using Db4oUnit;
using Db4oUnit.Tests;

namespace Db4oUnit.Tests
{
	public class TestLifeCycleTestCase : ITestCase
	{
		public virtual void TestLifeCycle()
		{
			TestSuite suite = new ReflectionTestSuiteBuilder(typeof(RunsLifeCycle)).Build();
			FrameworkTestCase.RunTestAndExpect(suite, 1);
			Assert.IsTrue(GetTestSubject(suite).TearDownCalled());
		}

		private RunsLifeCycle GetTestSubject(TestSuite suite)
		{
			return ((RunsLifeCycle)((TestMethod)suite.GetTests()[0]).GetSubject());
		}
	}
}
