namespace Db4objects.Db4o.Internal.Btree
{
	internal class BTreeRangeKeyIterator : Db4objects.Db4o.Internal.Btree.AbstractBTreeRangeIterator
	{
		public BTreeRangeKeyIterator(Db4objects.Db4o.Internal.Btree.BTreeRangeSingle range
			) : base(range)
		{
		}

		public override object Current
		{
			get
			{
				return CurrentPointer().Key();
			}
		}
	}
}
