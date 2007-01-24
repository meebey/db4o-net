namespace Db4objects.Db4o.Tests.Common.IO
{
	public class AllTests : Db4oUnit.ITestSuiteBuilder
	{
		public virtual Db4oUnit.TestSuite Build()
		{
			return new Db4oUnit.ReflectionTestSuiteBuilder(new System.Type[] { typeof(Db4objects.Db4o.Tests.Common.IO.IoAdapterTest)
				 }).Build();
		}

		public static void Main(string[] args)
		{
			new Db4oUnit.TestRunner(typeof(Db4objects.Db4o.Tests.Common.IO.AllTests)).Run();
		}
	}
}
