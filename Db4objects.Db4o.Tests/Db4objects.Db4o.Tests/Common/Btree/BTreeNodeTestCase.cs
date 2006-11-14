namespace Db4objects.Db4o.Tests.Common.Btree
{
	public class BTreeNodeTestCase : Db4objects.Db4o.Tests.Common.Btree.BTreeTestCaseBase
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Btree.BTreeNodeTestCase().RunSolo();
		}

		private readonly int[] keys = new int[] { -5, -4, -3, -2, -1, 0, 1, 2, 3, 4, 7, 9
			 };

		protected override void Db4oSetupAfterStore()
		{
			base.Db4oSetupAfterStore();
			Add(keys);
			Commit();
		}

		public virtual void TestLastKeyIndex()
		{
			Db4objects.Db4o.Inside.Btree.BTreeNode node = Node(3);
			Db4oUnit.Assert.AreEqual(1, node.LastKeyIndex(Trans()));
			Db4objects.Db4o.Transaction trans = NewTransaction();
			_btree.Add(trans, 5);
			Db4oUnit.Assert.AreEqual(1, node.LastKeyIndex(Trans()));
			_btree.Commit(trans);
			Db4oUnit.Assert.AreEqual(2, node.LastKeyIndex(Trans()));
		}

		private Db4objects.Db4o.Inside.Btree.BTreeNode Node(int value)
		{
			Db4objects.Db4o.Inside.Btree.IBTreeRange range = Search(value);
			System.Collections.IEnumerator i = range.Pointers();
			i.MoveNext();
			Db4objects.Db4o.Inside.Btree.BTreePointer firstPointer = (Db4objects.Db4o.Inside.Btree.BTreePointer
				)i.Current;
			Db4objects.Db4o.Inside.Btree.BTreeNode node = firstPointer.Node();
			node.DebugLoadFully(SystemTrans());
			return node;
		}

		public virtual void TestLastPointer()
		{
			Db4objects.Db4o.Inside.Btree.BTreeNode node = Node(3);
			Db4objects.Db4o.Inside.Btree.BTreePointer lastPointer = node.LastPointer(Trans());
			AssertPointerKey(4, lastPointer);
		}
	}
}
