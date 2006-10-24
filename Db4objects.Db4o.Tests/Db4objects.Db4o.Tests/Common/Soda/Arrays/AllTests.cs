namespace Db4objects.Db4o.Tests.Common.Soda.Arrays
{
	public class AllTests : Db4oUnit.Extensions.Db4oTestSuite
	{
		protected override System.Type[] TestCases()
		{
			return new System.Type[] { typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrMixedTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringOTestCase), typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringONTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringTTestCase), typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringTNTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringUTestCase), typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringUNTestCase)
				 };
		}

		public static void Main(string[] args)
		{
			new Db4oUnit.TestRunner(new Db4oUnit.Extensions.Db4oTestSuiteBuilder(new Db4oUnit.Extensions.Fixtures.Db4oSolo
				(), typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.AllTests))).Run();
		}
	}
}
