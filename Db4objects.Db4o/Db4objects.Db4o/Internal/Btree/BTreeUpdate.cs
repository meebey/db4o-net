namespace Db4objects.Db4o.Internal.Btree
{
	public abstract class BTreeUpdate : Db4objects.Db4o.Internal.Btree.BTreePatch
	{
		protected Db4objects.Db4o.Internal.Btree.BTreeUpdate _next;

		public BTreeUpdate(Db4objects.Db4o.Internal.Transaction transaction, object obj) : 
			base(transaction, obj)
		{
		}

		protected virtual bool HasNext()
		{
			return _next != null;
		}

		public override Db4objects.Db4o.Internal.Btree.BTreePatch ForTransaction(Db4objects.Db4o.Internal.Transaction
			 trans)
		{
			if (_transaction == trans)
			{
				return this;
			}
			if (_next == null)
			{
				return null;
			}
			return _next.ForTransaction(trans);
		}

		public virtual Db4objects.Db4o.Internal.Btree.BTreeUpdate RemoveFor(Db4objects.Db4o.Internal.Transaction
			 trans)
		{
			if (_transaction == trans)
			{
				return _next;
			}
			if (_next == null)
			{
				return this;
			}
			return _next.RemoveFor(trans);
		}

		public virtual void Append(Db4objects.Db4o.Internal.Btree.BTreeUpdate patch)
		{
			if (_transaction == patch._transaction)
			{
				throw new System.ArgumentException();
			}
			if (!HasNext())
			{
				_next = patch;
			}
			else
			{
				_next.Append(patch);
			}
		}

		protected virtual void ApplyKeyChange(object obj)
		{
			_object = obj;
			if (HasNext())
			{
				_next.ApplyKeyChange(obj);
			}
		}

		protected abstract void Committed(Db4objects.Db4o.Internal.Btree.BTree btree);

		public override object Commit(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.Btree.BTree
			 btree)
		{
			Db4objects.Db4o.Internal.Btree.BTreeUpdate patch = (Db4objects.Db4o.Internal.Btree.BTreeUpdate
				)ForTransaction(trans);
			if (patch is Db4objects.Db4o.Internal.Btree.BTreeCancelledRemoval)
			{
				object obj = patch.GetCommittedObject();
				ApplyKeyChange(obj);
			}
			return InternalCommit(trans, btree);
		}

		protected virtual object InternalCommit(Db4objects.Db4o.Internal.Transaction trans
			, Db4objects.Db4o.Internal.Btree.BTree btree)
		{
			if (_transaction == trans)
			{
				Committed(btree);
				if (HasNext())
				{
					return _next;
				}
				return GetCommittedObject();
			}
			if (HasNext())
			{
				SetNextIfPatch(_next.InternalCommit(trans, btree));
			}
			return this;
		}

		private void SetNextIfPatch(object newNext)
		{
			if (newNext is Db4objects.Db4o.Internal.Btree.BTreeUpdate)
			{
				_next = (Db4objects.Db4o.Internal.Btree.BTreeUpdate)newNext;
			}
			else
			{
				_next = null;
			}
		}

		protected abstract object GetCommittedObject();

		public override object Rollback(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.Btree.BTree
			 btree)
		{
			if (_transaction == trans)
			{
				if (HasNext())
				{
					return _next;
				}
				return GetObject();
			}
			if (HasNext())
			{
				SetNextIfPatch(_next.Rollback(trans, btree));
			}
			return this;
		}

		public override object Key(Db4objects.Db4o.Internal.Transaction trans)
		{
			Db4objects.Db4o.Internal.Btree.BTreePatch patch = ForTransaction(trans);
			if (patch == null)
			{
				return GetObject();
			}
			if (patch.IsRemove())
			{
				return Db4objects.Db4o.Foundation.No4.INSTANCE;
			}
			return patch.GetObject();
		}
	}
}
