namespace Db4objects.Db4o.Tests.Common.Btree
{
	public class BTreeAddRemoveTestCase : Db4objects.Db4o.Tests.Common.Btree.BTreeTestCaseBase
	{
		public virtual void TestFirstPointerMultiTransactional()
		{
			int count = BTREE_NODE_SIZE + 1;
			for (int i = 0; i < count; i++)
			{
				Add(count + i + 1);
			}
			int smallest = count + 1;
			Db4objects.Db4o.Internal.Transaction trans = NewTransaction();
			for (int i = 0; i < count; i++)
			{
				Add(trans, i);
			}
			Db4objects.Db4o.Internal.Btree.BTreePointer firstPointer = _btree.FirstPointer(Trans
				());
			AssertPointerKey(smallest, firstPointer);
		}

		public virtual void TestSingleRemoveAdd()
		{
			int element = 1;
			Add(element);
			AssertSize(1);
			Remove(element);
			AssertSize(0);
			Add(element);
			AssertSingleElement(element);
		}

		public virtual void TestSearchingRemoved()
		{
			int[] keys = new int[] { 3, 4, 7, 9 };
			Add(keys);
			Remove(4);
			Db4objects.Db4o.Internal.Btree.IBTreeRange result = Search(4);
			Db4oUnit.Assert.IsTrue(result.IsEmpty());
			Db4objects.Db4o.Internal.Btree.IBTreeRange range = result.Greater();
			Db4objects.Db4o.Tests.Common.Btree.BTreeAssert.AssertRange(new int[] { 7, 9 }, range
				);
		}

		public virtual void TestMultipleRemoveAdds()
		{
			int element = 1;
			Add(element);
			Remove(element);
			Remove(element);
			Add(element);
			AssertSingleElement(element);
		}

		public virtual void TestMultiTransactionCancelledRemoval()
		{
			int element = 1;
			Add(element);
			Commit();
			Db4objects.Db4o.Internal.Transaction trans1 = NewTransaction();
			Db4objects.Db4o.Internal.Transaction trans2 = NewTransaction();
			Remove(trans1, element);
			AssertSingleElement(trans2, element);
			Add(trans1, element);
			AssertSingleElement(trans1, element);
			AssertSingleElement(trans2, element);
			trans1.Commit();
			AssertSingleElement(element);
		}

		public virtual void TestMultiTransactionSearch()
		{
			int[] keys = new int[] { 3, 4, 7, 9 };
			Add(Trans(), keys);
			Commit(Trans());
			int[] assorted = new int[] { 1, 2, 11, 13, 21, 52, 51, 66, 89, 10 };
			Add(SystemTrans(), assorted);
			AssertKeys(keys);
			Remove(SystemTrans(), assorted);
			AssertKeys(keys);
			Db4objects.Db4o.Tests.Common.Btree.BTreeAssert.AssertRange(new int[] { 7, 9 }, Search
				(Trans(), 4).Greater());
		}

		private void AssertKeys(int[] keys)
		{
			Db4objects.Db4o.Tests.Common.Btree.BTreeAssert.AssertKeys(Trans(), _btree, keys);
		}

		public virtual void TestAddRemoveInDifferentTransactions()
		{
			int element = 1;
			Add(Trans(), element);
			Add(SystemTrans(), element);
			Remove(SystemTrans(), element);
			Remove(Trans(), element);
			AssertEmpty(SystemTrans());
			AssertEmpty(Trans());
		}

		public virtual void TestRemoveAddInDifferentTransactions()
		{
			int element = 1;
			Add(element);
			Db().Commit();
			Remove(Trans(), element);
			Remove(SystemTrans(), element);
			AssertEmpty(SystemTrans());
			AssertEmpty(Trans());
			Add(Trans(), element);
			AssertSingleElement(Trans(), element);
			Add(SystemTrans(), element);
			AssertSingleElement(SystemTrans(), element);
		}

		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Btree.BTreeAddRemoveTestCase().RunSolo();
		}
	}
}
