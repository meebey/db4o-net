namespace Db4objects.Db4o.Tests.Common.Btree
{
	public class BTreeSearchTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase, Db4oUnit.Extensions.Fixtures.IOptOutDefragSolo
		, Db4oUnit.Extensions.Fixtures.IOptOutCS
	{
		protected const int BTREE_NODE_SIZE = 4;

		public static void Main(string[] arguments)
		{
			new Db4objects.Db4o.Tests.Common.Btree.BTreeSearchTestCase().RunSolo();
		}

		public virtual void Test()
		{
			CycleIntKeys(new int[] { 3, 5, 5, 5, 7, 10, 11, 12, 12, 14 });
			CycleIntKeys(new int[] { 3, 5, 5, 5, 5, 7, 10, 11, 12, 12, 14 });
			CycleIntKeys(new int[] { 3, 5, 5, 5, 5, 5, 7, 10, 11, 12, 12, 14 });
			CycleIntKeys(new int[] { 3, 3, 5, 5, 5, 7, 10, 11, 12, 12, 14, 14 });
			CycleIntKeys(new int[] { 3, 3, 3, 5, 5, 5, 7, 10, 11, 12, 12, 14, 14, 14 });
		}

		private void CycleIntKeys(int[] values)
		{
			Db4objects.Db4o.Internal.Btree.BTree btree = Db4objects.Db4o.Tests.Common.Btree.BTreeAssert
				.CreateIntKeyBTree(Stream(), 0, BTREE_NODE_SIZE);
			for (int i = 0; i < 5; i++)
			{
				btree = CycleIntKeys(btree, values);
			}
		}

		private Db4objects.Db4o.Internal.Btree.BTree CycleIntKeys(Db4objects.Db4o.Internal.Btree.BTree
			 btree, int[] values)
		{
			for (int i = 0; i < values.Length; i++)
			{
				btree.Add(Trans(), values[i]);
			}
			ExpectKeysSearch(Trans(), btree, values);
			btree.Commit(Trans());
			int id = btree.GetID();
			Stream().Commit();
			Reopen();
			btree = Db4objects.Db4o.Tests.Common.Btree.BTreeAssert.CreateIntKeyBTree(Stream()
				, id, BTREE_NODE_SIZE);
			ExpectKeysSearch(Trans(), btree, values);
			for (int i = 0; i < values.Length; i++)
			{
				btree.Remove(Trans(), values[i]);
			}
			Db4objects.Db4o.Tests.Common.Btree.BTreeAssert.AssertEmpty(Trans(), btree);
			btree.Commit(Trans());
			Db4objects.Db4o.Tests.Common.Btree.BTreeAssert.AssertEmpty(Trans(), btree);
			return btree;
		}

		private void ExpectKeysSearch(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.Btree.BTree
			 btree, int[] keys)
		{
			int lastValue = int.MinValue;
			for (int i = 0; i < keys.Length; i++)
			{
				if (keys[i] != lastValue)
				{
					Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor expectingVisitor = Db4objects.Db4o.Tests.Common.Btree.BTreeAssert
						.CreateExpectingVisitor(keys[i], Db4objects.Db4o.Tests.Common.Foundation.IntArrays4
						.Occurences(keys, keys[i]));
					Db4objects.Db4o.Internal.Btree.IBTreeRange range = btree.Search(trans, keys[i]);
					Db4objects.Db4o.Tests.Common.Btree.BTreeAssert.TraverseKeys(range, expectingVisitor
						);
					expectingVisitor.AssertExpectations();
					lastValue = keys[i];
				}
			}
		}
	}
}
