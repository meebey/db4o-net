/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Fieldindex;
using Db4objects.Db4o.Internal.Query.Processor;

namespace Db4objects.Db4o.Internal.Fieldindex
{
	public abstract class IndexedNodeBase : IIndexedNode
	{
		protected readonly QConObject _constraint;

		public IndexedNodeBase(QConObject qcon)
		{
			if (null == qcon)
			{
				throw new ArgumentNullException();
			}
			if (null == qcon.GetField())
			{
				throw new ArgumentException();
			}
			_constraint = qcon;
		}

		public virtual TreeInt ToTreeInt()
		{
			return AddToTree(null, this);
		}

		public BTree GetIndex()
		{
			return GetYapField().GetIndex(Transaction());
		}

		private FieldMetadata GetYapField()
		{
			return _constraint.GetField().GetYapField();
		}

		public virtual QCon Constraint()
		{
			return _constraint;
		}

		public virtual bool IsResolved()
		{
			QCon parent = Constraint().Parent();
			return null == parent || !parent.HasParent();
		}

		public virtual IBTreeRange Search(object value)
		{
			return GetYapField().Search(Transaction(), value);
		}

		public static TreeInt AddToTree(TreeInt tree, IIndexedNode node)
		{
			IEnumerator i = node.GetEnumerator();
			while (i.MoveNext())
			{
				FieldIndexKey composite = (FieldIndexKey)i.Current;
				tree = (TreeInt)Tree.Add(tree, new TreeInt(composite.ParentID()));
			}
			return tree;
		}

		public virtual IIndexedNode Resolve()
		{
			if (IsResolved())
			{
				return null;
			}
			return IndexedPath.NewParentPath(this, Constraint());
		}

		private Db4objects.Db4o.Internal.Transaction Transaction()
		{
			return Constraint().Transaction();
		}

		public abstract IEnumerator GetEnumerator();

		public abstract void MarkAsBestIndex();

		public abstract int ResultSize();
	}
}
