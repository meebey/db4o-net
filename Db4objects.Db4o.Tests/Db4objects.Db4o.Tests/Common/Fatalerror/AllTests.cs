namespace Db4objects.Db4o.Tests.Common.Fatalerror
{
	public class AllTests : Db4oUnit.Extensions.Db4oTestSuite
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Fatalerror.AllTests().RunSoloAndClientServer();
		}

		protected override System.Type[] TestCases()
		{
			return new System.Type[] { typeof(Db4objects.Db4o.Tests.Common.Fatalerror.FatalExceptionInNestedCallTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Fatalerror.NativeQueryTestCase) };
		}
	}
}
