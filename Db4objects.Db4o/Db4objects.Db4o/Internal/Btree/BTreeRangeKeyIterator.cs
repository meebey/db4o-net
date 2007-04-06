using Db4objects.Db4o.Internal.Btree;

namespace Db4objects.Db4o.Internal.Btree
{
	internal class BTreeRangeKeyIterator : AbstractBTreeRangeIterator
	{
		public BTreeRangeKeyIterator(BTreeRangeSingle range) : base(range)
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
