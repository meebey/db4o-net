using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Btree.Algebra;

namespace Db4objects.Db4o.Internal.Btree.Algebra
{
	/// <exclude></exclude>
	public class BTreeRangeUnionUnion : BTreeRangeUnionOperation
	{
		public BTreeRangeUnionUnion(BTreeRangeUnion union) : base(union)
		{
		}

		protected override IBTreeRange Execute(BTreeRangeUnion union)
		{
			return BTreeAlgebra.Union(_union, union);
		}

		protected override IBTreeRange Execute(BTreeRangeSingle single)
		{
			return BTreeAlgebra.Union(_union, single);
		}
	}
}
