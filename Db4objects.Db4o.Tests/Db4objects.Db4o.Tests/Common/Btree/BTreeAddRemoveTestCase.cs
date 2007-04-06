using Db4oUnit;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Tests.Common.Btree;

namespace Db4objects.Db4o.Tests.Common.Btree
{
	public class BTreeAddRemoveTestCase : BTreeTestCaseBase
	{
		public virtual void TestFirstPointerMultiTransactional()
		{
			int count = BTREE_NODE_SIZE + 1;
			for (int i = 0; i < count; i++)
			{
				Add(count + i + 1);
			}
			int smallest = count + 1;
			Transaction trans = NewTransaction();
			for (int i = 0; i < count; i++)
			{
				Add(trans, i);
			}
			BTreePointer firstPointer = _btree.FirstPointer(Trans());
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
			IBTreeRange result = Search(4);
			Assert.IsTrue(result.IsEmpty());
			IBTreeRange range = result.Greater();
			BTreeAssert.AssertRange(new int[] { 7, 9 }, range);
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
			Transaction trans1 = NewTransaction();
			Transaction trans2 = NewTransaction();
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
			BTreeAssert.AssertRange(new int[] { 7, 9 }, Search(Trans(), 4).Greater());
		}

		private void AssertKeys(int[] keys)
		{
			BTreeAssert.AssertKeys(Trans(), _btree, keys);
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

		public virtual void TestAddAddRollbackCommmitInDifferentTransactions()
		{
			int element = 1;
			Add(Trans(), element);
			Add(SystemTrans(), element);
			Db().Rollback();
			AssertSingleElement(SystemTrans(), element);
			Db().Commit();
			AssertSingleElement(Trans(), element);
			AssertSingleElement(SystemTrans(), element);
		}

		public static void Main(string[] args)
		{
			new BTreeAddRemoveTestCase().RunSolo();
		}
	}
}
