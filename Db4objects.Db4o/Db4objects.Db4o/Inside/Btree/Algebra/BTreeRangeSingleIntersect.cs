namespace Db4objects.Db4o.Inside.Btree.Algebra
{
	/// <exclude></exclude>
	public class BTreeRangeSingleIntersect : Db4objects.Db4o.Inside.Btree.Algebra.BTreeRangeSingleOperation
	{
		public BTreeRangeSingleIntersect(Db4objects.Db4o.Inside.Btree.BTreeRangeSingle single
			) : base(single)
		{
		}

		protected override Db4objects.Db4o.Inside.Btree.IBTreeRange Execute(Db4objects.Db4o.Inside.Btree.BTreeRangeSingle
			 single)
		{
			return Db4objects.Db4o.Inside.Btree.Algebra.BTreeAlgebra.Intersect(_single, single
				);
		}

		protected override Db4objects.Db4o.Inside.Btree.IBTreeRange Execute(Db4objects.Db4o.Inside.Btree.BTreeRangeUnion
			 union)
		{
			return Db4objects.Db4o.Inside.Btree.Algebra.BTreeAlgebra.Intersect(union, _single
				);
		}
	}
}
