namespace Db4oUnit.Tests
{
	public class TestLifeCycleTestCase : Db4oUnit.ITestCase
	{
		public virtual void TestLifeCycle()
		{
			Db4oUnit.TestSuite suite = new Db4oUnit.ReflectionTestSuiteBuilder(typeof(Db4oUnit.Tests.RunsLifeCycle
				)).Build();
			Db4oUnit.Tests.FrameworkTestCase.RunTestAndExpect(suite, 1);
			Db4oUnit.Assert.IsTrue(GetTestSubject(suite).TearDownCalled());
		}

		private Db4oUnit.Tests.RunsLifeCycle GetTestSubject(Db4oUnit.TestSuite suite)
		{
			return ((Db4oUnit.Tests.RunsLifeCycle)((Db4oUnit.TestMethod)suite.GetTests()[0]).
				GetSubject());
		}
	}
}
