namespace Db4objects.Db4o.Tests.Common.Btree
{
	/// <exclude></exclude>
	public class BTreePointerTestCase : Db4objects.Db4o.Tests.Common.Btree.BTreeTestCaseBase
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Btree.BTreePointerTestCase().RunSolo();
		}

		private readonly int[] keys = new int[] { -5, -4, -3, -2, -1, 0, 1, 2, 3, 4, 7, 9
			 };

		protected override void Db4oSetupAfterStore()
		{
			base.Db4oSetupAfterStore();
			Add(keys);
			Commit();
		}

		public virtual void TestLastPointer()
		{
			Db4objects.Db4o.Internal.Btree.BTreePointer pointer = _btree.LastPointer(Trans());
			AssertPointerKey(9, pointer);
		}

		public virtual void TestPrevious()
		{
			Db4objects.Db4o.Internal.Btree.BTreePointer pointer = GetPointerForKey(3);
			Db4objects.Db4o.Internal.Btree.BTreePointer previousPointer = pointer.Previous();
			AssertPointerKey(2, previousPointer);
		}

		public virtual void TestNextOperatesInReadMode()
		{
			Db4objects.Db4o.Internal.Btree.BTreePointer pointer = _btree.FirstPointer(Trans()
				);
			AssertReadModePointerIteration(keys, pointer);
		}

		public virtual void TestSearchOperatesInReadMode()
		{
			Db4objects.Db4o.Internal.Btree.BTreePointer pointer = GetPointerForKey(3);
			AssertReadModePointerIteration(new int[] { 3, 4, 7, 9 }, pointer);
		}

		private Db4objects.Db4o.Internal.Btree.BTreePointer GetPointerForKey(int key)
		{
			Db4objects.Db4o.Internal.Btree.IBTreeRange range = Search(key);
			System.Collections.IEnumerator pointers = range.Pointers();
			Db4oUnit.Assert.IsTrue(pointers.MoveNext());
			Db4objects.Db4o.Internal.Btree.BTreePointer pointer = (Db4objects.Db4o.Internal.Btree.BTreePointer
				)pointers.Current;
			return pointer;
		}

		private void AssertReadModePointerIteration(int[] expectedKeys, Db4objects.Db4o.Internal.Btree.BTreePointer
			 pointer)
		{
			object[] expected = Db4objects.Db4o.Tests.Common.Foundation.IntArrays4.ToObjectArray
				(expectedKeys);
			for (int i = 0; i < expected.Length; i++)
			{
				Db4oUnit.Assert.IsNotNull(pointer, "Expected '" + expected[i] + "'");
				Db4oUnit.Assert.AreNotSame(_btree.Root(), pointer.Node());
				AssertInReadMode(pointer.Node());
				Db4oUnit.Assert.AreEqual(expected[i], pointer.Key());
				AssertInReadMode(pointer.Node());
				pointer = pointer.Next();
			}
		}

		private void AssertInReadMode(Db4objects.Db4o.Internal.Btree.BTreeNode node)
		{
			Db4oUnit.Assert.IsFalse(node.CanWrite());
		}

		protected override Db4objects.Db4o.Internal.Btree.BTree NewBTree()
		{
			return NewBTreeWithNoNodeCaching();
		}

		private Db4objects.Db4o.Internal.Btree.BTree NewBTreeWithNoNodeCaching()
		{
			return Db4objects.Db4o.Tests.Common.Btree.BTreeAssert.CreateIntKeyBTree(Stream(), 
				0, 0, BTREE_NODE_SIZE);
		}
	}
}
