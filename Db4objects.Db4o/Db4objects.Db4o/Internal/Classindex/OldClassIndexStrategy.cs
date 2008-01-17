/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Classindex;

namespace Db4objects.Db4o.Internal.Classindex
{
	/// <exclude></exclude>
	public class OldClassIndexStrategy : AbstractClassIndexStrategy, ITransactionParticipant
	{
		private ClassIndex _index;

		private readonly Hashtable4 _perTransaction = new Hashtable4();

		public OldClassIndexStrategy(ClassMetadata yapClass) : base(yapClass)
		{
		}

		public override void Read(ObjectContainerBase stream, int indexID)
		{
			_index = CreateClassIndex(stream);
			if (indexID > 0)
			{
				_index.SetID(indexID);
			}
			_index.SetStateDeactivated();
		}

		private ClassIndex GetActiveIndex(Transaction transaction)
		{
			if (null != _index)
			{
				_index.EnsureActive(transaction);
			}
			return _index;
		}

		public override int EntryCount(Transaction transaction)
		{
			if (_index != null)
			{
				return _index.EntryCount(transaction);
			}
			return 0;
		}

		public override void Initialize(ObjectContainerBase stream)
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

		public override int Write(Transaction transaction)
		{
			if (_index == null)
			{
				return 0;
			}
			_index.Write(transaction);
			return _index.GetID();
		}

		private void FlushContext(Transaction transaction)
		{
			OldClassIndexStrategy.TransactionState context = GetState(transaction);
			ClassIndex index = GetActiveIndex(transaction);
			context.TraverseAdded(new _IVisitor4_68(this, index));
			context.TraverseRemoved(new _IVisitor4_74(this, transaction, index));
		}

		private sealed class _IVisitor4_68 : IVisitor4
		{
			public _IVisitor4_68(OldClassIndexStrategy _enclosing, ClassIndex index)
			{
				this._enclosing = _enclosing;
				this.index = index;
			}

			public void Visit(object a_object)
			{
				index.Add(this._enclosing.IdFromValue(a_object));
			}

			private readonly OldClassIndexStrategy _enclosing;

			private readonly ClassIndex index;
		}

		private sealed class _IVisitor4_74 : IVisitor4
		{
			public _IVisitor4_74(OldClassIndexStrategy _enclosing, Transaction transaction, ClassIndex
				 index)
			{
				this._enclosing = _enclosing;
				this.transaction = transaction;
				this.index = index;
			}

			public void Visit(object a_object)
			{
				int id = this._enclosing.IdFromValue(a_object);
				ObjectReference yo = transaction.ReferenceForId(id);
				if (yo != null)
				{
					transaction.RemoveReference(yo);
				}
				index.Remove(id);
			}

			private readonly OldClassIndexStrategy _enclosing;

			private readonly Transaction transaction;

			private readonly ClassIndex index;
		}

		private void WriteIndex(Transaction transaction)
		{
			_index.SetStateDirty();
			_index.Write(transaction);
		}

		internal sealed class TransactionState
		{
			private Tree i_addToClassIndex;

			private Tree i_removeFromClassIndex;

			public void Add(int id)
			{
				i_removeFromClassIndex = Tree.RemoveLike(i_removeFromClassIndex, new TreeInt(id));
				i_addToClassIndex = Tree.Add(i_addToClassIndex, new TreeInt(id));
			}

			public void Remove(int id)
			{
				i_addToClassIndex = Tree.RemoveLike(i_addToClassIndex, new TreeInt(id));
				i_removeFromClassIndex = Tree.Add(i_removeFromClassIndex, new TreeInt(id));
			}

			public void DontDelete(int id)
			{
				i_removeFromClassIndex = Tree.RemoveLike(i_removeFromClassIndex, new TreeInt(id));
			}

			internal void Traverse(Tree node, IVisitor4 visitor)
			{
				if (node != null)
				{
					node.Traverse(visitor);
				}
			}

			public void TraverseAdded(IVisitor4 visitor4)
			{
				Traverse(i_addToClassIndex, visitor4);
			}

			public void TraverseRemoved(IVisitor4 visitor4)
			{
				Traverse(i_removeFromClassIndex, visitor4);
			}
		}

		protected override void InternalAdd(Transaction transaction, int id)
		{
			GetState(transaction).Add(id);
		}

		private OldClassIndexStrategy.TransactionState GetState(Transaction transaction)
		{
			lock (_perTransaction)
			{
				OldClassIndexStrategy.TransactionState context = (OldClassIndexStrategy.TransactionState
					)_perTransaction.Get(transaction);
				if (null == context)
				{
					context = new OldClassIndexStrategy.TransactionState();
					_perTransaction.Put(transaction, context);
					((LocalTransaction)transaction).Enlist(this);
				}
				return context;
			}
		}

		private Tree GetAll(Transaction transaction)
		{
			ClassIndex ci = GetActiveIndex(transaction);
			if (ci == null)
			{
				return null;
			}
			Tree.ByRef tree = new Tree.ByRef(Tree.DeepClone(ci.GetRoot(), null));
			OldClassIndexStrategy.TransactionState context = GetState(transaction);
			context.TraverseAdded(new _IVisitor4_150(this, tree));
			context.TraverseRemoved(new _IVisitor4_155(this, tree));
			return tree.value;
		}

		private sealed class _IVisitor4_150 : IVisitor4
		{
			public _IVisitor4_150(OldClassIndexStrategy _enclosing, Tree.ByRef tree)
			{
				this._enclosing = _enclosing;
				this.tree = tree;
			}

			public void Visit(object obj)
			{
				tree.value = Tree.Add(tree.value, new TreeInt(this._enclosing.IdFromValue(obj)));
			}

			private readonly OldClassIndexStrategy _enclosing;

			private readonly Tree.ByRef tree;
		}

		private sealed class _IVisitor4_155 : IVisitor4
		{
			public _IVisitor4_155(OldClassIndexStrategy _enclosing, Tree.ByRef tree)
			{
				this._enclosing = _enclosing;
				this.tree = tree;
			}

			public void Visit(object obj)
			{
				tree.value = Tree.RemoveLike(tree.value, (TreeInt)obj);
			}

			private readonly OldClassIndexStrategy _enclosing;

			private readonly Tree.ByRef tree;
		}

		protected override void InternalRemove(Transaction transaction, int id)
		{
			GetState(transaction).Remove(id);
		}

		public override void TraverseAll(Transaction transaction, IVisitor4 command)
		{
			Tree tree = GetAll(transaction);
			if (tree != null)
			{
				tree.Traverse(new _IVisitor4_170(this, command));
			}
		}

		private sealed class _IVisitor4_170 : IVisitor4
		{
			public _IVisitor4_170(OldClassIndexStrategy _enclosing, IVisitor4 command)
			{
				this._enclosing = _enclosing;
				this.command = command;
			}

			public void Visit(object obj)
			{
				command.Visit(this._enclosing.IdFromValue(obj));
			}

			private readonly OldClassIndexStrategy _enclosing;

			private readonly IVisitor4 command;
		}

		public virtual int IdFromValue(object value)
		{
			return ((TreeInt)value)._key;
		}

		private ClassIndex CreateClassIndex(ObjectContainerBase stream)
		{
			if (stream.IsClient())
			{
				return new ClassIndexClient(_yapClass);
			}
			return new ClassIndex(_yapClass);
		}

		public override void DontDelete(Transaction transaction, int id)
		{
			GetState(transaction).DontDelete(id);
		}

		public virtual void Commit(Transaction trans)
		{
			if (null != _index)
			{
				FlushContext(trans);
				WriteIndex(trans);
			}
		}

		public virtual void Dispose(Transaction transaction)
		{
			lock (_perTransaction)
			{
				_perTransaction.Remove(transaction);
			}
		}

		public virtual void Rollback(Transaction transaction)
		{
		}

		public override void DefragReference(ClassMetadata yapClass, DefragmentContextImpl
			 context, int classIndexID)
		{
		}

		public override int Id()
		{
			// nothing to do
			return _index.GetID();
		}

		public override IEnumerator AllSlotIDs(Transaction trans)
		{
			throw new NotImplementedException();
		}

		public override void DefragIndex(DefragmentContextImpl context)
		{
		}
	}
}
