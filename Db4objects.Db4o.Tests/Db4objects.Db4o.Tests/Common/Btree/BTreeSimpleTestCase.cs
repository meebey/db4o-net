namespace Db4objects.Db4o.Tests.Common.Btree
{
	public class BTreeSimpleTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase, Db4oUnit.Extensions.Fixtures.IOptOutDefragSolo
		, Db4oUnit.Extensions.Fixtures.IOptOutCS
	{
		protected const int BTREE_NODE_SIZE = 4;

		internal int[] _keys = { 3, 234, 55, 87, 2, 1, 101, 59, 70, 300, 288 };

		internal int[] _values;

		internal int[] _sortedKeys = { 1, 2, 3, 55, 59, 70, 87, 101, 234, 288, 300 };

		internal int[] _sortedValues;

		internal int[] _keysOnRemoval = { 1, 2, 55, 59, 70, 87, 234, 288, 300 };

		internal int[] _valuesOnRemoval;

		internal int[] _one = { 1 };

		internal int[] _none = {  };

		public BTreeSimpleTestCase() : base()
		{
			_values = new int[_keys.Length];
			for (int i = 0; i < _keys.Length; i++)
			{
				_values[i] = _keys[i];
			}
			_sortedValues = new int[_sortedKeys.Length];
			for (int i = 0; i < _sortedKeys.Length; i++)
			{
				_sortedValues[i] = _sortedKeys[i];
			}
			_valuesOnRemoval = new int[_keysOnRemoval.Length];
			for (int i = 0; i < _keysOnRemoval.Length; i++)
			{
				_valuesOnRemoval[i] = _keysOnRemoval[i];
			}
		}

		public virtual void TestIntKeys()
		{
			Db4objects.Db4o.Internal.Btree.BTree btree = Db4objects.Db4o.Tests.Common.Btree.BTreeAssert
				.CreateIntKeyBTree(Stream(), 0, BTREE_NODE_SIZE);
			for (int i = 0; i < 5; i++)
			{
				btree = CycleIntKeys(btree);
			}
		}

		private Db4objects.Db4o.Internal.Btree.BTree CycleIntKeys(Db4objects.Db4o.Internal.Btree.BTree
			 btree)
		{
			AddKeys(btree);
			ExpectKeys(btree, _sortedKeys);
			btree.Commit(Trans());
			ExpectKeys(btree, _sortedKeys);
			RemoveKeys(btree);
			ExpectKeys(btree, _keysOnRemoval);
			btree.Rollback(Trans());
			ExpectKeys(btree, _sortedKeys);
			int id = btree.GetID();
			Reopen();
			btree = Db4objects.Db4o.Tests.Common.Btree.BTreeAssert.CreateIntKeyBTree(Stream()
				, id, BTREE_NODE_SIZE);
			ExpectKeys(btree, _sortedKeys);
			RemoveKeys(btree);
			ExpectKeys(btree, _keysOnRemoval);
			btree.Commit(Trans());
			ExpectKeys(btree, _keysOnRemoval);
			for (int i = 1; i < _keysOnRemoval.Length; i++)
			{
				btree.Remove(Trans(), _keysOnRemoval[i]);
			}
			ExpectKeys(btree, _one);
			btree.Commit(Trans());
			ExpectKeys(btree, _one);
			btree.Remove(Trans(), 1);
			btree.Rollback(Trans());
			ExpectKeys(btree, _one);
			btree.Remove(Trans(), 1);
			btree.Commit(Trans());
			ExpectKeys(btree, _none);
			return btree;
		}

		private void RemoveKeys(Db4objects.Db4o.Internal.Btree.BTree btree)
		{
			btree.Remove(Trans(), 3);
			btree.Remove(Trans(), 101);
		}

		private void AddKeys(Db4objects.Db4o.Internal.Btree.BTree btree)
		{
			Db4objects.Db4o.Internal.Transaction trans = Trans();
			for (int i = 0; i < _keys.Length; i++)
			{
				btree.Add(trans, _keys[i]);
			}
		}

		private void ExpectKeys(Db4objects.Db4o.Internal.Btree.BTree btree, int[] keys)
		{
			Db4objects.Db4o.Tests.Common.Btree.BTreeAssert.AssertKeys(Trans(), btree, keys);
		}
	}
}
