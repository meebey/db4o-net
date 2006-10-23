namespace Db4objects.Db4o.Inside.Btree
{
	/// <exclude></exclude>
	public class BTreeAdd : Db4objects.Db4o.Inside.Btree.BTreePatch
	{
		public BTreeAdd(Db4objects.Db4o.Transaction transaction, object obj) : base(transaction
			, obj)
		{
		}

		protected virtual object RolledBack(Db4objects.Db4o.Inside.Btree.BTree btree)
		{
			btree.NotifyRemoveListener(GetObject());
			return Db4objects.Db4o.Foundation.No4.INSTANCE;
		}

		public override string ToString()
		{
			return "(+) " + base.ToString();
		}

		public override object Commit(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.Inside.Btree.BTree
			 btree)
		{
			if (_transaction == trans)
			{
				return GetObject();
			}
			return this;
		}

		public override Db4objects.Db4o.Inside.Btree.BTreePatch ForTransaction(Db4objects.Db4o.Transaction
			 trans)
		{
			if (_transaction == trans)
			{
				return this;
			}
			return null;
		}

		public override object Key(Db4objects.Db4o.Transaction trans)
		{
			if (_transaction != trans)
			{
				return Db4objects.Db4o.Foundation.No4.INSTANCE;
			}
			return GetObject();
		}

		public override object Rollback(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.Inside.Btree.BTree
			 btree)
		{
			if (_transaction == trans)
			{
				return RolledBack(btree);
			}
			return this;
		}

		public override bool IsAdd()
		{
			return true;
		}
	}
}