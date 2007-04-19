using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.IX;

namespace Db4objects.Db4o.Internal.IX
{
	/// <exclude></exclude>
	public abstract class IxTree : Tree, IVisitor4
	{
		internal IndexTransaction _fieldTransaction;

		internal int _version;

		internal int _nodes = 1;

		internal IxTree(IndexTransaction a_ft)
		{
			_fieldTransaction = a_ft;
			_version = a_ft.i_version;
		}

		public override Tree Add(Tree a_new, int a_cmp)
		{
			if (a_cmp < 0)
			{
				if (_subsequent == null)
				{
					_subsequent = a_new;
				}
				else
				{
					_subsequent = _subsequent.Add(a_new);
				}
			}
			else
			{
				if (_preceding == null)
				{
					_preceding = a_new;
				}
				else
				{
					_preceding = _preceding.Add(a_new);
				}
			}
			return BalanceCheckNulls();
		}

		internal virtual void BeginMerge()
		{
			_preceding = null;
			_subsequent = null;
			SetSizeOwn();
		}

		public override object DeepClone(object a_param)
		{
			Db4objects.Db4o.Internal.IX.IxTree tree = (Db4objects.Db4o.Internal.IX.IxTree)this
				.ShallowClone();
			tree._fieldTransaction = (IndexTransaction)a_param;
			tree._nodes = _nodes;
			return tree;
		}

		internal IIndexable4 Handler()
		{
			return _fieldTransaction.i_index._handler;
		}

		internal Index4 Index()
		{
			return _fieldTransaction.i_index;
		}

		/// <summary>
		/// Overridden in IxFileRange
		/// Only call directly after compare()
		/// </summary>
		internal virtual int[] LowerAndUpperMatch()
		{
			return null;
		}

		public sealed override int Nodes()
		{
			return _nodes;
		}

		public override void SetSizeOwn()
		{
			base.SetSizeOwn();
			_nodes = 1;
		}

		public override void SetSizeOwnPrecedingSubsequent()
		{
			base.SetSizeOwnPrecedingSubsequent();
			_nodes = 1 + _preceding.Nodes() + _subsequent.Nodes();
		}

		public override void SetSizeOwnPreceding()
		{
			base.SetSizeOwnPreceding();
			_nodes = 1 + _preceding.Nodes();
		}

		public override void SetSizeOwnSubsequent()
		{
			base.SetSizeOwnSubsequent();
			_nodes = 1 + _subsequent.Nodes();
		}

		public sealed override void SetSizeOwnPlus(Tree tree)
		{
			base.SetSizeOwnPlus(tree);
			_nodes = 1 + tree.Nodes();
		}

		public sealed override void SetSizeOwnPlus(Tree tree1, Tree tree2)
		{
			base.SetSizeOwnPlus(tree1, tree2);
			_nodes = 1 + tree1.Nodes() + tree2.Nodes();
		}

		internal virtual int SlotLength()
		{
			return Handler().LinkLength() + Const4.INT_LENGTH;
		}

		internal LocalObjectContainer Stream()
		{
			return Trans().File();
		}

		internal LocalTransaction Trans()
		{
			return _fieldTransaction.i_trans;
		}

		public abstract void Visit(object obj);

		public abstract void Visit(IVisitor4 visitor, int[] a_lowerAndUpperMatch);

		public abstract void VisitAll(IIntObjectVisitor visitor);

		public abstract void FreespaceVisit(FreespaceVisitor visitor, int index);

		public abstract int Write(IIndexable4 a_handler, StatefulBuffer a_writer);

		public virtual void VisitFirst(FreespaceVisitor visitor)
		{
			if (_preceding != null)
			{
				((Db4objects.Db4o.Internal.IX.IxTree)_preceding).VisitFirst(visitor);
				if (visitor.Visited())
				{
					return;
				}
			}
			FreespaceVisit(visitor, 0);
			if (visitor.Visited())
			{
				return;
			}
			if (_subsequent != null)
			{
				((Db4objects.Db4o.Internal.IX.IxTree)_subsequent).VisitFirst(visitor);
				if (visitor.Visited())
				{
					return;
				}
			}
		}

		public virtual void VisitLast(FreespaceVisitor visitor)
		{
			if (_subsequent != null)
			{
				((Db4objects.Db4o.Internal.IX.IxTree)_subsequent).VisitLast(visitor);
				if (visitor.Visited())
				{
					return;
				}
			}
			FreespaceVisit(visitor, 0);
			if (visitor.Visited())
			{
				return;
			}
			if (_preceding != null)
			{
				((Db4objects.Db4o.Internal.IX.IxTree)_preceding).VisitLast(visitor);
				if (visitor.Visited())
				{
					return;
				}
			}
		}

		protected override Tree ShallowCloneInternal(Tree tree)
		{
			Db4objects.Db4o.Internal.IX.IxTree ixTree = (Db4objects.Db4o.Internal.IX.IxTree)base
				.ShallowCloneInternal(tree);
			ixTree._fieldTransaction = _fieldTransaction;
			ixTree._version = _version;
			ixTree._nodes = _nodes;
			return ixTree;
		}

		public override object Key()
		{
			throw new NotImplementedException();
		}
	}
}
