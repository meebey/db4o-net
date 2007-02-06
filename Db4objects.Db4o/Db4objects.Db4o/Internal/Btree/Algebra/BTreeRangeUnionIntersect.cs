namespace Db4objects.Db4o.Internal.Btree.Algebra
{
	/// <exclude></exclude>
	public class BTreeRangeUnionIntersect : Db4objects.Db4o.Internal.Btree.Algebra.BTreeRangeUnionOperation
	{
		public BTreeRangeUnionIntersect(Db4objects.Db4o.Internal.Btree.BTreeRangeUnion union
			) : base(union)
		{
		}

		protected override Db4objects.Db4o.Internal.Btree.IBTreeRange Execute(Db4objects.Db4o.Internal.Btree.BTreeRangeSingle
			 range)
		{
			return Db4objects.Db4o.Internal.Btree.Algebra.BTreeAlgebra.Intersect(_union, range
				);
		}

		protected override Db4objects.Db4o.Internal.Btree.IBTreeRange Execute(Db4objects.Db4o.Internal.Btree.BTreeRangeUnion
			 union)
		{
			return Db4objects.Db4o.Internal.Btree.Algebra.BTreeAlgebra.Intersect(_union, union
				);
		}
	}
}
