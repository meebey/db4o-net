namespace Db4objects.Db4o.Inside.Fieldindex
{
	/// <exclude></exclude>
	public class IndexedLeaf : Db4objects.Db4o.Inside.Fieldindex.IndexedNodeBase, Db4objects.Db4o.Inside.Fieldindex.IIndexedNodeWithRange
	{
		private readonly Db4objects.Db4o.Inside.Btree.IBTreeRange _range;

		public IndexedLeaf(Db4objects.Db4o.QConObject qcon) : base(qcon)
		{
			_range = Search();
		}

		private Db4objects.Db4o.Inside.Btree.IBTreeRange Search()
		{
			Db4objects.Db4o.Inside.Btree.IBTreeRange range = Search(Constraint().GetObject());
			Db4objects.Db4o.Inside.Fieldindex.QEBitmap bitmap = Db4objects.Db4o.Inside.Fieldindex.QEBitmap
				.ForQE(Constraint().Evaluator());
			if (bitmap.TakeGreater())
			{
				if (bitmap.TakeEqual())
				{
					return range.ExtendToLast();
				}
				Db4objects.Db4o.Inside.Btree.IBTreeRange greater = range.Greater();
				if (bitmap.TakeSmaller())
				{
					return greater.Union(range.Smaller());
				}
				return greater;
			}
			if (bitmap.TakeSmaller())
			{
				if (bitmap.TakeEqual())
				{
					return range.ExtendToFirst();
				}
				return range.Smaller();
			}
			return range;
		}

		public override int ResultSize()
		{
			return _range.Size();
		}

		public override System.Collections.IEnumerator GetEnumerator()
		{
			return _range.Keys();
		}

		public virtual Db4objects.Db4o.Inside.Btree.IBTreeRange GetRange()
		{
			return _range;
		}
	}
}
