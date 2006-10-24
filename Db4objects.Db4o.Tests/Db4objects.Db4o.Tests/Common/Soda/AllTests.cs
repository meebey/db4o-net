namespace Db4objects.Db4o.Tests.Common.Soda
{
	public class AllTests : Db4oUnit.Extensions.Db4oTestSuite
	{
		protected override System.Type[] TestCases()
		{
			return new System.Type[] { typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.AllTests)
				 };
		}

		public static void Main(string[] args)
		{
			new Db4oUnit.TestRunner(new Db4oUnit.Extensions.Db4oTestSuiteBuilder(new Db4oUnit.Extensions.Fixtures.Db4oSolo
				(), typeof(Db4objects.Db4o.Tests.Common.Soda.AllTests))).Run();
		}
	}
}
