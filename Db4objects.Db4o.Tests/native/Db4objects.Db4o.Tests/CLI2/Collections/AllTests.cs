namespace Db4objects.Db4o.Tests.CLI2.Collections
{
	public class AllTests : Db4oUnit.Extensions.Db4oTestSuite
	{
		protected override System.Type[] TestCases()
		{
#if NET_2_0 || CF_2_0
			return new System.Type[] { typeof(GenericDictionaryTestCase) };
#else
			return new System.Type[0];
#endif
		}
	}
}
