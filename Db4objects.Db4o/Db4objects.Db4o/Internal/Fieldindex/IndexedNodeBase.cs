namespace Db4objects.Db4o.Internal.Fieldindex
{
	public abstract class IndexedNodeBase : Db4objects.Db4o.Internal.Fieldindex.IIndexedNode
	{
		private readonly Db4objects.Db4o.Internal.Query.Processor.QConObject _constraint;

		public IndexedNodeBase(Db4objects.Db4o.Internal.Query.Processor.QConObject qcon)
		{
			if (null == qcon)
			{
				throw new System.ArgumentNullException();
			}
			if (null == qcon.GetField())
			{
				throw new System.ArgumentException();
			}
			_constraint = qcon;
		}

		public virtual Db4objects.Db4o.Internal.TreeInt ToTreeInt()
		{
			return AddToTree(null, this);
		}

		public Db4objects.Db4o.Internal.Btree.BTree GetIndex()
		{
			return GetYapField().GetIndex(Transaction());
		}

		private Db4objects.Db4o.Internal.FieldMetadata GetYapField()
		{
			return _constraint.GetField().GetYapField();
		}

		public virtual Db4objects.Db4o.Internal.Query.Processor.QCon Constraint()
		{
			return _constraint;
		}

		public virtual bool IsResolved()
		{
			Db4objects.Db4o.Internal.Query.Processor.QCon parent = Constraint().Parent();
			return null == parent || !parent.HasParent();
		}

		public virtual Db4objects.Db4o.Internal.Btree.IBTreeRange Search(object value)
		{
			return GetYapField().Search(Transaction(), value);
		}

		public static Db4objects.Db4o.Internal.TreeInt AddToTree(Db4objects.Db4o.Internal.TreeInt
			 tree, Db4objects.Db4o.Internal.Fieldindex.IIndexedNode node)
		{
			System.Collections.IEnumerator i = node.GetEnumerator();
			while (i.MoveNext())
			{
				Db4objects.Db4o.Internal.Btree.FieldIndexKey composite = (Db4objects.Db4o.Internal.Btree.FieldIndexKey
					)i.Current;
				tree = (Db4objects.Db4o.Internal.TreeInt)Db4objects.Db4o.Foundation.Tree.Add(tree
					, new Db4objects.Db4o.Internal.TreeInt(composite.ParentID()));
			}
			return tree;
		}

		public virtual Db4objects.Db4o.Internal.Fieldindex.IIndexedNode Resolve()
		{
			if (IsResolved())
			{
				return null;
			}
			return Db4objects.Db4o.Internal.Fieldindex.IndexedPath.NewParentPath(this, Constraint
				());
		}

		private Db4objects.Db4o.Internal.Transaction Transaction()
		{
			return Constraint().Transaction();
		}

		public abstract System.Collections.IEnumerator GetEnumerator();

		public abstract int ResultSize();
	}
}
