namespace Db4objects.Db4o.Internal.Fieldindex
{
	/// <exclude></exclude>
	public class IndexedLeaf : Db4objects.Db4o.Internal.Fieldindex.IndexedNodeBase, Db4objects.Db4o.Internal.Fieldindex.IIndexedNodeWithRange
	{
		private readonly Db4objects.Db4o.Internal.Btree.IBTreeRange _range;

		public IndexedLeaf(Db4objects.Db4o.Internal.Query.Processor.QConObject qcon) : base
			(qcon)
		{
			_range = Search();
		}

		private Db4objects.Db4o.Internal.Btree.IBTreeRange Search()
		{
			Db4objects.Db4o.Internal.Btree.IBTreeRange range = Search(Constraint().GetObject(
				));
			Db4objects.Db4o.Internal.Fieldindex.QEBitmap bitmap = Db4objects.Db4o.Internal.Fieldindex.QEBitmap
				.ForQE(Constraint().Evaluator());
			if (bitmap.TakeGreater())
			{
				if (bitmap.TakeEqual())
				{
					return range.ExtendToLast();
				}
				Db4objects.Db4o.Internal.Btree.IBTreeRange greater = range.Greater();
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

		public virtual Db4objects.Db4o.Internal.Btree.IBTreeRange GetRange()
		{
			return _range;
		}
	}
}
