namespace Db4objects.Db4o.Tests.Common.Acid
{
	public class AllTests : Db4oUnit.Extensions.Db4oTestSuite
	{
		protected override System.Type[] TestCases()
		{
			return new System.Type[] { typeof(Db4objects.Db4o.Tests.Common.Acid.CrashSimulatingTestCase)
				 };
		}
	}
}
