namespace Db4objects.Db4o.Inside.Fieldindex
{
	public abstract class IndexedNodeBase : Db4objects.Db4o.Inside.Fieldindex.IIndexedNode
	{
		private readonly Db4objects.Db4o.QConObject _constraint;

		public IndexedNodeBase(Db4objects.Db4o.QConObject qcon)
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

		public virtual Db4objects.Db4o.TreeInt ToTreeInt()
		{
			return AddToTree(null, this);
		}

		public Db4objects.Db4o.Inside.Btree.BTree GetIndex()
		{
			return GetYapField().GetIndex(Transaction());
		}

		private Db4objects.Db4o.YapField GetYapField()
		{
			return _constraint.GetField().GetYapField();
		}

		public virtual Db4objects.Db4o.QCon Constraint()
		{
			return _constraint;
		}

		public virtual bool IsResolved()
		{
			Db4objects.Db4o.QCon parent = Constraint().Parent();
			return null == parent || !parent.HasParent();
		}

		public virtual Db4objects.Db4o.Inside.Btree.IBTreeRange Search(object value)
		{
			return GetYapField().Search(Transaction(), value);
		}

		public static Db4objects.Db4o.TreeInt AddToTree(Db4objects.Db4o.TreeInt tree, Db4objects.Db4o.Inside.Fieldindex.IIndexedNode
			 node)
		{
			System.Collections.IEnumerator i = node.GetEnumerator();
			while (i.MoveNext())
			{
				Db4objects.Db4o.Inside.Btree.FieldIndexKey composite = (Db4objects.Db4o.Inside.Btree.FieldIndexKey
					)i.Current;
				tree = (Db4objects.Db4o.TreeInt)Db4objects.Db4o.Foundation.Tree.Add(tree, new Db4objects.Db4o.TreeInt
					(composite.ParentID()));
			}
			return tree;
		}

		public virtual Db4objects.Db4o.Inside.Fieldindex.IIndexedNode Resolve()
		{
			if (IsResolved())
			{
				return null;
			}
			return Db4objects.Db4o.Inside.Fieldindex.IndexedPath.NewParentPath(this, Constraint
				());
		}

		private Db4objects.Db4o.Transaction Transaction()
		{
			return Constraint().Transaction();
		}

		public abstract System.Collections.IEnumerator GetEnumerator();

		public abstract int ResultSize();
	}
}
