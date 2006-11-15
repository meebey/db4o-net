namespace Db4objects.Db4o.Inside.Btree
{
	internal class BTreeRangeKeyIterator : Db4objects.Db4o.Inside.Btree.AbstractBTreeRangeIterator
	{
		public BTreeRangeKeyIterator(Db4objects.Db4o.Inside.Btree.BTreeRangeSingle range)
			 : base(range)
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
