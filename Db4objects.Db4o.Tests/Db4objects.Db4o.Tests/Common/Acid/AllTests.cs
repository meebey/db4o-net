namespace Db4objects.Db4o.Tests.Common.Acid
{
	public class AllTests : Db4oUnit.Extensions.Db4oTestSuite
	{
		public static int Main(string[] args)
		{
			return new Db4objects.Db4o.Tests.Common.Acid.AllTests().RunSolo();
		}

		protected override System.Type[] TestCases()
		{
			return new System.Type[] { typeof(Db4objects.Db4o.Tests.Common.Acid.CrashSimulatingTestCase)
				 };
		}
	}
}
