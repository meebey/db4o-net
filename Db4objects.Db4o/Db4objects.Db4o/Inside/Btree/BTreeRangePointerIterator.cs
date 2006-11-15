namespace Db4objects.Db4o.Inside.Btree
{
	public class BTreeRangePointerIterator : Db4objects.Db4o.Inside.Btree.AbstractBTreeRangeIterator
	{
		public BTreeRangePointerIterator(Db4objects.Db4o.Inside.Btree.BTreeRangeSingle range
			) : base(range)
		{
		}

		public override object Current
		{
			get
			{
				return CurrentPointer();
			}
		}
	}
}
