namespace Db4objects.Db4o.Tests.Common.Querying
{
	public class AllTests : Db4oUnit.Extensions.Db4oTestSuite
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Querying.AllTests().RunSoloAndClientServer();
		}

		protected override System.Type[] TestCases()
		{
			return new System.Type[] { typeof(Db4objects.Db4o.Tests.Common.Querying.CascadedDeleteUpdate)
				, typeof(Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteArray), typeof(Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteDeleted)
				, typeof(Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteFalse), typeof(Db4objects.Db4o.Tests.Common.Querying.CascadeOnActivate)
				, typeof(Db4objects.Db4o.Tests.Common.Querying.CascadeOnDelete), typeof(Db4objects.Db4o.Tests.Common.Querying.CascadeOnUpdate)
				, typeof(Db4objects.Db4o.Tests.Common.Querying.CascadeToArray), typeof(Db4objects.Db4o.Tests.Common.Querying.IdListQueryResultTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Querying.IndexedQueriesTestCase), typeof(Db4objects.Db4o.Tests.Common.Querying.LazyQueryResultTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Querying.MultiFieldIndexQueryTestCase), typeof(Db4objects.Db4o.Tests.Common.Querying.ObjectSetTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Querying.OrderedQueryTestCase) };
		}
	}
}
