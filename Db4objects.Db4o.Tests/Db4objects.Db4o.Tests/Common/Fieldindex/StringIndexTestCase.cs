namespace Db4objects.Db4o.Tests.Common.Fieldindex
{
	/// <exclude></exclude>
	public class StringIndexTestCase : Db4objects.Db4o.Tests.Common.Fieldindex.StringIndexTestCaseBase
		, Db4oUnit.Extensions.Fixtures.IOptOutCS
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Fieldindex.StringIndexTestCase().RunSolo();
		}

		public virtual void TestNotEquals()
		{
			Add("foo");
			Add("bar");
			Add("baz");
			Add(null);
			Db4objects.Db4o.Query.IQuery query = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Fieldindex.StringIndexTestCaseBase.Item)
				);
			query.Descend("name").Constrain("bar").Not();
			AssertItems(new string[] { "foo", "baz", null }, query.Execute());
		}

		public virtual void TestCancelRemovalRollback()
		{
			PrepareCancelRemoval(Trans(), "original");
			Rename("original", "updated");
			Db().Rollback();
			GrafittiFreeSpace();
			Reopen();
			AssertExists("original");
		}

		public virtual void TestCancelRemovalRollbackForMultipleTransactions()
		{
			Db4objects.Db4o.Internal.Transaction trans1 = NewTransaction();
			Db4objects.Db4o.Internal.Transaction trans2 = NewTransaction();
			PrepareCancelRemoval(trans1, "original");
			AssertExists(trans2, "original");
			trans1.Rollback();
			AssertExists(trans2, "original");
			Add(trans2, "second");
			AssertExists(trans2, "original");
			trans2.Commit();
			AssertExists(trans2, "original");
			GrafittiFreeSpace();
			Reopen();
			AssertExists("original");
		}

		public virtual void TestCancelRemoval()
		{
			PrepareCancelRemoval(Trans(), "original");
			Db().Commit();
			GrafittiFreeSpace();
			Reopen();
			AssertExists("original");
		}

		private void PrepareCancelRemoval(Db4objects.Db4o.Internal.Transaction transaction
			, string itemName)
		{
			Add(itemName);
			Db().Commit();
			Rename(transaction, itemName, "updated");
			AssertExists(transaction, "updated");
			Rename(transaction, "updated", itemName);
			AssertExists(transaction, itemName);
		}

		public virtual void TestCancelRemovalForMultipleTransactions()
		{
			Db4objects.Db4o.Internal.Transaction trans1 = NewTransaction();
			Db4objects.Db4o.Internal.Transaction trans2 = NewTransaction();
			PrepareCancelRemoval(trans1, "original");
			Rename(trans2, "original", "updated");
			trans1.Commit();
			GrafittiFreeSpace();
			Reopen();
			AssertExists("original");
		}

		public virtual void TestDeletingAndReaddingMember()
		{
			Add("original");
			AssertExists("original");
			Rename("original", "updated");
			AssertExists("updated");
			Db4oUnit.Assert.IsNull(Query("original"));
			Reopen();
			AssertExists("updated");
			Db4oUnit.Assert.IsNull(Query("original"));
		}
	}
}
