namespace Db4objects.Db4o.Tests.Common.Btree
{
	public class BTreeRollbackTestCase : Db4objects.Db4o.Tests.Common.Btree.BTreeTestCaseBase
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Btree.BTreeRollbackTestCase().RunSolo();
		}

		private static readonly int[] COMMITTED_VALUES = new int[] { 6, 8, 15, 45, 43, 9, 
			23, 25, 7, 3, 2 };

		private static readonly int[] ROLLED_BACK_VALUES = new int[] { 16, 18, 115, 19, 17
			, 13, 12 };

		public virtual void Test()
		{
			Add(COMMITTED_VALUES);
			CommitBTree();
			for (int i = 0; i < 5; i++)
			{
				Add(ROLLED_BACK_VALUES);
				RollbackBTree();
			}
			Db4objects.Db4o.Tests.Common.Btree.BTreeAssert.AssertKeys(Trans(), _btree, COMMITTED_VALUES
				);
		}

		private void CommitBTree()
		{
			_btree.Commit(Trans());
			Trans().Commit();
		}

		private void RollbackBTree()
		{
			_btree.Rollback(Trans());
			Trans().Rollback();
		}
	}
}
