using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Btree.Algebra;

namespace Db4objects.Db4o.Internal.Btree.Algebra
{
	/// <exclude></exclude>
	public abstract class BTreeRangeSingleOperation : BTreeRangeOperation
	{
		protected readonly BTreeRangeSingle _single;

		public BTreeRangeSingleOperation(BTreeRangeSingle single)
		{
			_single = single;
		}
	}
}
