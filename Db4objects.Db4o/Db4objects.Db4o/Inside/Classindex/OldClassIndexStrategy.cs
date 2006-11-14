namespace Db4objects.Db4o.Inside.Classindex
{
	/// <exclude></exclude>
	public class OldClassIndexStrategy : Db4objects.Db4o.Inside.Classindex.AbstractClassIndexStrategy
		, Db4objects.Db4o.ITransactionParticipant
	{
		private Db4objects.Db4o.Inside.Classindex.ClassIndex _index;

		private readonly Db4objects.Db4o.Foundation.Hashtable4 _perTransaction = new Db4objects.Db4o.Foundation.Hashtable4
			();

		public OldClassIndexStrategy(Db4objects.Db4o.YapClass yapClass) : base(yapClass)
		{
		}

		public override void Read(Db4objects.Db4o.YapStream stream, int indexID)
		{
			_index = CreateClassIndex(stream);
			if (indexID > 0)
			{
				_index.SetID(indexID);
			}
			_index.SetStateDeactivated();
		}

		private Db4objects.Db4o.Inside.Classindex.ClassIndex GetActiveIndex(Db4objects.Db4o.Transaction
			 transaction)
		{
			if (null != _index)
			{
				_index.EnsureActive(transaction);
			}
			return _index;
		}

		public override int EntryCount(Db4objects.Db4o.Transaction transaction)
		{
			if (_index != null)
			{
				return _index.EntryCount(transaction);
			}
			return 0;
		}

		public override void Initialize(Db4objects.Db4o.YapStream stream)
		{
			_index = CreateClassIndex(stream);
		}

		public override void Purge()
		{
			if (_index != null)
			{
				if (!_index.IsDirty())
				{
					_index.Clear();
					_index.SetStateDeactivated();
				}
			}
		}

		public override int Write(Db4objects.Db4o.Transaction transaction)
		{
			if (_index == null)
			{
				return 0;
			}
			_index.Write(transaction);
			return _index.GetID();
		}

		private void FlushContext(Db4objects.Db4o.Transaction transaction)
		{
			Db4objects.Db4o.Inside.Classindex.OldClassIndexStrategy.TransactionState context = 
				GetState(transaction);
			Db4objects.Db4o.Inside.Classindex.ClassIndex index = GetActiveIndex(transaction);
			context.TraverseAdded(new _AnonymousInnerClass68(this, index));
			context.TraverseRemoved(new _AnonymousInnerClass74(this, transaction, index));
		}

		private sealed class _AnonymousInnerClass68 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass68(OldClassIndexStrategy _enclosing, Db4objects.Db4o.Inside.Classindex.ClassIndex
				 index)
			{
				this._enclosing = _enclosing;
				this.index = index;
			}

			public void Visit(object a_object)
			{
				index.Add(this._enclosing.IdFromValue(a_object));
			}

			private readonly OldClassIndexStrategy _enclosing;

			private readonly Db4objects.Db4o.Inside.Classindex.ClassIndex index;
		}

		private sealed class _AnonymousInnerClass74 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass74(OldClassIndexStrategy _enclosing, Db4objects.Db4o.Transaction
				 transaction, Db4objects.Db4o.Inside.Classindex.ClassIndex index)
			{
				this._enclosing = _enclosing;
				this.transaction = transaction;
				this.index = index;
			}

			public void Visit(object a_object)
			{
				int id = this._enclosing.IdFromValue(a_object);
				Db4objects.Db4o.YapStream stream = transaction.Stream();
				Db4objects.Db4o.YapObject yo = stream.GetYapObject(id);
				if (yo != null)
				{
					stream.RemoveReference(yo);
				}
				index.Remove(id);
			}

			private readonly OldClassIndexStrategy _enclosing;

			private readonly Db4objects.Db4o.Transaction transaction;

			private readonly Db4objects.Db4o.Inside.Classindex.ClassIndex index;
		}

		private void WriteIndex(Db4objects.Db4o.Transaction transaction)
		{
			_index.SetStateDirty();
			_index.Write(transaction);
		}

		internal sealed class TransactionState
		{
			private Db4objects.Db4o.Foundation.Tree i_addToClassIndex;

			private Db4objects.Db4o.Foundation.Tree i_removeFromClassIndex;

			public void Add(int id)
			{
				i_removeFromClassIndex = Db4objects.Db4o.Foundation.Tree.RemoveLike(i_removeFromClassIndex
					, new Db4objects.Db4o.TreeInt(id));
				i_addToClassIndex = Db4objects.Db4o.Foundation.Tree.Add(i_addToClassIndex, new Db4objects.Db4o.TreeInt
					(id));
			}

			public void Remove(int id)
			{
				i_addToClassIndex = Db4objects.Db4o.Foundation.Tree.RemoveLike(i_addToClassIndex, 
					new Db4objects.Db4o.TreeInt(id));
				i_removeFromClassIndex = Db4objects.Db4o.Foundation.Tree.Add(i_removeFromClassIndex
					, new Db4objects.Db4o.TreeInt(id));
			}

			public void DontDelete(int id)
			{
				i_removeFromClassIndex = Db4objects.Db4o.Foundation.Tree.RemoveLike(i_removeFromClassIndex
					, new Db4objects.Db4o.TreeInt(id));
			}

			internal void Traverse(Db4objects.Db4o.Foundation.Tree node, Db4objects.Db4o.Foundation.IVisitor4
				 visitor)
			{
				if (node != null)
				{
					node.Traverse(visitor);
				}
			}

			public void TraverseAdded(Db4objects.Db4o.Foundation.IVisitor4 visitor4)
			{
				Traverse(i_addToClassIndex, visitor4);
			}

			public void TraverseRemoved(Db4objects.Db4o.Foundation.IVisitor4 visitor4)
			{
				Traverse(i_removeFromClassIndex, visitor4);
			}
		}

		protected override void InternalAdd(Db4objects.Db4o.Transaction transaction, int 
			id)
		{
			GetState(transaction).Add(id);
		}

		private Db4objects.Db4o.Inside.Classindex.OldClassIndexStrategy.TransactionState 
			GetState(Db4objects.Db4o.Transaction transaction)
		{
			lock (_perTransaction)
			{
				Db4objects.Db4o.Inside.Classindex.OldClassIndexStrategy.TransactionState context = 
					(Db4objects.Db4o.Inside.Classindex.OldClassIndexStrategy.TransactionState)_perTransaction
					.Get(transaction);
				if (null == context)
				{
					context = new Db4objects.Db4o.Inside.Classindex.OldClassIndexStrategy.TransactionState
						();
					_perTransaction.Put(transaction, context);
					transaction.Enlist(this);
				}
				return context;
			}
		}

		private Db4objects.Db4o.Foundation.Tree GetAll(Db4objects.Db4o.Transaction transaction
			)
		{
			Db4objects.Db4o.Inside.Classindex.ClassIndex ci = GetActiveIndex(transaction);
			if (ci == null)
			{
				return null;
			}
			Db4objects.Db4o.Foundation.Tree[] tree = new Db4objects.Db4o.Foundation.Tree[] { 
				Db4objects.Db4o.Foundation.Tree.DeepClone(ci.GetRoot(), null) };
			Db4objects.Db4o.Inside.Classindex.OldClassIndexStrategy.TransactionState context = 
				GetState(transaction);
			context.TraverseAdded(new _AnonymousInnerClass151(this, tree));
			context.TraverseRemoved(new _AnonymousInnerClass156(this, tree));
			return tree[0];
		}

		private sealed class _AnonymousInnerClass151 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass151(OldClassIndexStrategy _enclosing, Db4objects.Db4o.Foundation.Tree[]
				 tree)
			{
				this._enclosing = _enclosing;
				this.tree = tree;
			}

			public void Visit(object obj)
			{
				tree[0] = Db4objects.Db4o.Foundation.Tree.Add(tree[0], new Db4objects.Db4o.TreeInt
					(this._enclosing.IdFromValue(obj)));
			}

			private readonly OldClassIndexStrategy _enclosing;

			private readonly Db4objects.Db4o.Foundation.Tree[] tree;
		}

		private sealed class _AnonymousInnerClass156 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass156(OldClassIndexStrategy _enclosing, Db4objects.Db4o.Foundation.Tree[]
				 tree)
			{
				this._enclosing = _enclosing;
				this.tree = tree;
			}

			public void Visit(object obj)
			{
				tree[0] = Db4objects.Db4o.Foundation.Tree.RemoveLike(tree[0], (Db4objects.Db4o.TreeInt
					)obj);
			}

			private readonly OldClassIndexStrategy _enclosing;

			private readonly Db4objects.Db4o.Foundation.Tree[] tree;
		}

		protected override void InternalRemove(Db4objects.Db4o.Transaction transaction, int
			 id)
		{
			GetState(transaction).Remove(id);
		}

		public override void TraverseAll(Db4objects.Db4o.Transaction transaction, Db4objects.Db4o.Foundation.IVisitor4
			 command)
		{
			Db4objects.Db4o.Foundation.Tree tree = GetAll(transaction);
			if (tree != null)
			{
				tree.Traverse(new _AnonymousInnerClass171(this, command));
			}
		}

		private sealed class _AnonymousInnerClass171 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass171(OldClassIndexStrategy _enclosing, Db4objects.Db4o.Foundation.IVisitor4
				 command)
			{
				this._enclosing = _enclosing;
				this.command = command;
			}

			public void Visit(object obj)
			{
				command.Visit(this._enclosing.IdFromValue(obj));
			}

			private readonly OldClassIndexStrategy _enclosing;

			private readonly Db4objects.Db4o.Foundation.IVisitor4 command;
		}

		public virtual int IdFromValue(object value)
		{
			return ((Db4objects.Db4o.TreeInt)value)._key;
		}

		private Db4objects.Db4o.Inside.Classindex.ClassIndex CreateClassIndex(Db4objects.Db4o.YapStream
			 stream)
		{
			if (stream.IsClient())
			{
				return new Db4objects.Db4o.Inside.Classindex.ClassIndexClient(_yapClass);
			}
			return new Db4objects.Db4o.Inside.Classindex.ClassIndex(_yapClass);
		}

		public override void DontDelete(Db4objects.Db4o.Transaction transaction, int id)
		{
			GetState(transaction).DontDelete(id);
		}

		public virtual void Commit(Db4objects.Db4o.Transaction trans)
		{
			if (null != _index)
			{
				FlushContext(trans);
				WriteIndex(trans);
			}
		}

		public virtual void Dispose(Db4objects.Db4o.Transaction transaction)
		{
			lock (_perTransaction)
			{
				_perTransaction.Remove(transaction);
			}
		}

		public virtual void Rollback(Db4objects.Db4o.Transaction transaction)
		{
		}

		public override void DefragReference(Db4objects.Db4o.YapClass yapClass, Db4objects.Db4o.ReaderPair
			 readers, int classIndexID)
		{
		}

		public override int Id()
		{
			return _index.GetID();
		}

		public override System.Collections.IEnumerator AllSlotIDs(Db4objects.Db4o.Transaction
			 trans)
		{
			throw new System.NotImplementedException();
		}

		public override void DefragIndex(Db4objects.Db4o.ReaderPair readers)
		{
		}
	}
}
