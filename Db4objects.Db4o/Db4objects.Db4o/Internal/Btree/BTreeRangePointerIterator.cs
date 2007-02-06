namespace Db4objects.Db4o.Internal.Btree
{
	public class BTreeRangePointerIterator : Db4objects.Db4o.Internal.Btree.AbstractBTreeRangeIterator
	{
		public BTreeRangePointerIterator(Db4objects.Db4o.Internal.Btree.BTreeRangeSingle 
			range) : base(range)
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
