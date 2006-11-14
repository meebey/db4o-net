namespace Db4objects.Db4o.Inside.Btree
{
	public abstract class BTreePatch
	{
		protected readonly Db4objects.Db4o.Transaction _transaction;

		protected object _object;

		public BTreePatch(Db4objects.Db4o.Transaction transaction, object obj)
		{
			_transaction = transaction;
			_object = obj;
		}

		public abstract object Commit(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.Inside.Btree.BTree
			 btree);

		public virtual bool IsRemove()
		{
			return false;
		}

		public abstract Db4objects.Db4o.Inside.Btree.BTreePatch ForTransaction(Db4objects.Db4o.Transaction
			 trans);

		public virtual object GetObject()
		{
			return _object;
		}

		public abstract object Rollback(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.Inside.Btree.BTree
			 btree);

		public override string ToString()
		{
			if (_object == null)
			{
				return "[NULL]";
			}
			return _object.ToString();
		}

		public virtual bool IsAdd()
		{
			return false;
		}

		public abstract object Key(Db4objects.Db4o.Transaction trans);
	}
}
