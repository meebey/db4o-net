namespace Db4oUnit.Tests
{
	public class AllTests : Db4oUnit.ITestSuiteBuilder
	{
		public virtual Db4oUnit.TestSuite Build()
		{
			return new Db4oUnit.ReflectionTestSuiteBuilder(new System.Type[] { typeof(Db4oUnit.Tests.FrameworkTestCase
				), typeof(Db4oUnit.Tests.AssertTestCase), typeof(Db4oUnit.Tests.TestLifeCycleTestCase
				), typeof(Db4oUnit.Tests.ReflectionTestSuiteBuilderTestCase), typeof(Db4oUnit.Tests.ReinstantiatePerMethodTest
				) }).Build();
		}

		public static void Main(string[] args)
		{
			new Db4oUnit.TestRunner(typeof(Db4oUnit.Tests.AllTests)).Run();
		}
	}
}
