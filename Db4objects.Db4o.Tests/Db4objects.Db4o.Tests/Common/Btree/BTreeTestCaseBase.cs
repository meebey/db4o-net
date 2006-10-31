namespace Db4objects.Db4o.Tests.Common.Btree
{
	public abstract class BTreeTestCaseBase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		protected const int BTREE_NODE_SIZE = 4;

		protected Db4objects.Db4o.Inside.Btree.BTree _btree;

		protected override void Db4oSetupAfterStore()
		{
			_btree = NewBTree();
		}

		protected virtual Db4objects.Db4o.Inside.Btree.BTree NewBTree()
		{
			return Db4objects.Db4o.Tests.Common.Btree.BTreeAssert.CreateIntKeyBTree(Stream(), 
				0, BTREE_NODE_SIZE);
		}

		protected virtual Db4objects.Db4o.Inside.Btree.IBTreeRange Range(int lower, int upper
			)
		{
			Db4objects.Db4o.Inside.Btree.IBTreeRange lowerRange = Search(lower);
			Db4objects.Db4o.Inside.Btree.IBTreeRange upperRange = Search(upper);
			return lowerRange.ExtendToLastOf(upperRange);
		}

		protected virtual Db4objects.Db4o.Inside.Btree.IBTreeRange Search(int key)
		{
			return Search(Trans(), key);
		}

		protected virtual void Add(int[] keys)
		{
			for (int i = 0; i < keys.Length; ++i)
			{
				Add(keys[i]);
			}
		}

		protected virtual Db4objects.Db4o.Inside.Btree.IBTreeRange Search(Db4objects.Db4o.Transaction
			 trans, int key)
		{
			return _btree.Search(trans, key);
		}

		protected virtual void Commit(Db4objects.Db4o.Transaction trans)
		{
			_btree.Commit(trans);
		}

		protected virtual void Commit()
		{
			Commit(Trans());
		}

		protected virtual void Remove(Db4objects.Db4o.Transaction transaction, int[] keys
			)
		{
			for (int i = 0; i < keys.Length; i++)
			{
				Remove(transaction, keys[i]);
			}
		}

		protected virtual void Add(Db4objects.Db4o.Transaction transaction, int[] keys)
		{
			for (int i = 0; i < keys.Length; i++)
			{
				Add(transaction, keys[i]);
			}
		}

		protected virtual void AssertEmpty(Db4objects.Db4o.Transaction transaction)
		{
			Db4objects.Db4o.Tests.Common.Btree.BTreeAssert.AssertEmpty(transaction, _btree);
		}

		protected virtual void Add(Db4objects.Db4o.Transaction transaction, int element)
		{
			_btree.Add(transaction, element);
		}

		protected virtual void Remove(int element)
		{
			Remove(Trans(), element);
		}

		protected virtual void Remove(Db4objects.Db4o.Transaction trans, int element)
		{
			_btree.Remove(trans, element);
		}

		protected virtual void Add(int element)
		{
			Add(Trans(), element);
		}

		private int Size()
		{
			return _btree.Size(Trans());
		}

		protected virtual void AssertSize(int expected)
		{
			Db4oUnit.Assert.AreEqual(expected, Size());
		}

		protected virtual void AssertSingleElement(int element)
		{
			AssertSingleElement(Trans(), element);
		}

		protected virtual void AssertSingleElement(Db4objects.Db4o.Transaction trans, int
			 element)
		{
			Db4objects.Db4o.Tests.Common.Btree.BTreeAssert.AssertSingleElement(trans, _btree, 
				element);
		}

		protected virtual void AssertPointerKey(int key, Db4objects.Db4o.Inside.Btree.BTreePointer
			 pointer)
		{
			Db4oUnit.Assert.AreEqual(key, pointer.Key());
		}
	}
}
