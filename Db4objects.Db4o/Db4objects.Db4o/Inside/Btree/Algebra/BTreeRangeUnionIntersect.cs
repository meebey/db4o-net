namespace Db4objects.Db4o.Inside.Btree.Algebra
{
	/// <exclude></exclude>
	public class BTreeRangeUnionIntersect : Db4objects.Db4o.Inside.Btree.Algebra.BTreeRangeUnionOperation
	{
		public BTreeRangeUnionIntersect(Db4objects.Db4o.Inside.Btree.BTreeRangeUnion union
			) : base(union)
		{
		}

		protected override Db4objects.Db4o.Inside.Btree.IBTreeRange Execute(Db4objects.Db4o.Inside.Btree.BTreeRangeSingle
			 range)
		{
			return Db4objects.Db4o.Inside.Btree.Algebra.BTreeAlgebra.Intersect(_union, range);
		}

		protected override Db4objects.Db4o.Inside.Btree.IBTreeRange Execute(Db4objects.Db4o.Inside.Btree.BTreeRangeUnion
			 union)
		{
			return Db4objects.Db4o.Inside.Btree.Algebra.BTreeAlgebra.Intersect(_union, union);
		}
	}
}
