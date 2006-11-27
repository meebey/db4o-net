namespace Db4objects.Db4o.Tests.CLI2.Assorted
{
	public class AllTests : Db4oUnit.Extensions.Db4oTestSuite
	{
		protected override System.Type[] TestCases()
		{
			return new System.Type[] {
				typeof(Db4objects.Db4o.Tests.CLI2.Assorted.NullableTypes),
				typeof(SimpleGenericTypeTestCase),
			};
		}
	}
}
