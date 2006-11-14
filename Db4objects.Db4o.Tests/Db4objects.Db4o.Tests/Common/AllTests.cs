namespace Db4objects.Db4o.Tests.Common
{
	public class AllTests : Db4oUnit.Extensions.Db4oTestSuite
	{
		protected override System.Type[] TestCases()
		{
			return new System.Type[] { typeof(Db4objects.Db4o.Tests.Common.Acid.AllTests), typeof(Db4objects.Db4o.Tests.Common.Assorted.AllTests)
				, typeof(Db4objects.Db4o.Tests.Common.Btree.AllTests), typeof(Db4objects.Db4o.Tests.Common.Classindex.AllTests)
				, typeof(Db4objects.Db4o.Tests.Common.Defragment.AllTests), typeof(Db4objects.Db4o.Tests.Common.Fieldindex.AllTests)
				, typeof(Db4objects.Db4o.Tests.Common.Foundation.AllTests), typeof(Db4objects.Db4o.Tests.Common.Handlers.AllTests)
				, typeof(Db4objects.Db4o.Tests.Common.Header.AllTests), typeof(Db4objects.Db4o.Tests.Common.Reflect.AllTests)
				, typeof(Db4objects.Db4o.Tests.Common.Querying.AllTests), typeof(Db4objects.Db4o.Tests.Common.Soda.AllTests)
				, typeof(Db4objects.Db4o.Tests.Common.Types.AllTests) };
		}
	}
}
