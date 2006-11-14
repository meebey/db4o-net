namespace Db4objects.Db4o.Inside.Btree.Algebra
{
	/// <exclude></exclude>
	public abstract class BTreeRangeOperation : Db4objects.Db4o.Inside.Btree.IBTreeRangeVisitor
	{
		private Db4objects.Db4o.Inside.Btree.IBTreeRange _resultingRange;

		public BTreeRangeOperation() : base()
		{
		}

		public virtual Db4objects.Db4o.Inside.Btree.IBTreeRange Dispatch(Db4objects.Db4o.Inside.Btree.IBTreeRange
			 range)
		{
			range.Accept(this);
			return _resultingRange;
		}

		public void Visit(Db4objects.Db4o.Inside.Btree.BTreeRangeSingle single)
		{
			_resultingRange = Execute(single);
		}

		public void Visit(Db4objects.Db4o.Inside.Btree.BTreeRangeUnion union)
		{
			_resultingRange = Execute(union);
		}

		protected abstract Db4objects.Db4o.Inside.Btree.IBTreeRange Execute(Db4objects.Db4o.Inside.Btree.BTreeRangeUnion
			 union);

		protected abstract Db4objects.Db4o.Inside.Btree.IBTreeRange Execute(Db4objects.Db4o.Inside.Btree.BTreeRangeSingle
			 single);
	}
}
