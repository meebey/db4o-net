namespace Db4objects.Db4o.Inside.Fieldindex
{
	public class JoinedLeaf : Db4objects.Db4o.Inside.Fieldindex.IIndexedNodeWithRange
	{
		private readonly Db4objects.Db4o.QCon _constraint;

		private readonly Db4objects.Db4o.Inside.Fieldindex.IIndexedNodeWithRange _leaf1;

		private readonly Db4objects.Db4o.Inside.Btree.IBTreeRange _range;

		public JoinedLeaf(Db4objects.Db4o.QCon constraint, Db4objects.Db4o.Inside.Fieldindex.IIndexedNodeWithRange
			 leaf1, Db4objects.Db4o.Inside.Btree.IBTreeRange range)
		{
			if (null == constraint || null == leaf1 || null == range)
			{
				throw new System.ArgumentNullException();
			}
			_constraint = constraint;
			_leaf1 = leaf1;
			_range = range;
		}

		public virtual Db4objects.Db4o.QCon GetConstraint()
		{
			return _constraint;
		}

		public virtual Db4objects.Db4o.Inside.Btree.IBTreeRange GetRange()
		{
			return _range;
		}

		public virtual System.Collections.IEnumerator GetEnumerator()
		{
			return _range.Keys();
		}

		public virtual Db4objects.Db4o.TreeInt ToTreeInt()
		{
			return Db4objects.Db4o.Inside.Fieldindex.IndexedNodeBase.AddToTree(null, this);
		}

		public virtual Db4objects.Db4o.Inside.Btree.BTree GetIndex()
		{
			return _leaf1.GetIndex();
		}

		public virtual bool IsResolved()
		{
			return _leaf1.IsResolved();
		}

		public virtual Db4objects.Db4o.Inside.Fieldindex.IIndexedNode Resolve()
		{
			return Db4objects.Db4o.Inside.Fieldindex.IndexedPath.NewParentPath(this, _constraint
				);
		}

		public virtual int ResultSize()
		{
			return _range.Size();
		}
	}
}
