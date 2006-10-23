namespace Db4objects.Db4o.Tests.Common.Btree
{
	public class AllTests : Db4oUnit.Extensions.Db4oTestSuite
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Btree.AllTests().RunSolo();
		}

		protected override System.Type[] TestCases()
		{
			return new System.Type[] { typeof(Db4objects.Db4o.Tests.Common.Btree.BTreeAddRemoveTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Btree.BTreeFreeTestCase), typeof(Db4objects.Db4o.Tests.Common.Btree.BTreePointerTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Btree.BTreeRangeTestCase), typeof(Db4objects.Db4o.Tests.Common.Btree.BTreeRollbackTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Btree.BTreeSearchTestCase), typeof(Db4objects.Db4o.Tests.Common.Btree.BTreeSimpleTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Btree.SearcherLowestHighestTestCase), typeof(Db4objects.Db4o.Tests.Common.Btree.SearcherTestCase)
				 };
		}
	}
}
