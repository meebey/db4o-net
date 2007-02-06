namespace Db4objects.Db4o.Internal.Btree.Algebra
{
	/// <exclude></exclude>
	public class BTreeRangeUnionUnion : Db4objects.Db4o.Internal.Btree.Algebra.BTreeRangeUnionOperation
	{
		public BTreeRangeUnionUnion(Db4objects.Db4o.Internal.Btree.BTreeRangeUnion union)
			 : base(union)
		{
		}

		protected override Db4objects.Db4o.Internal.Btree.IBTreeRange Execute(Db4objects.Db4o.Internal.Btree.BTreeRangeUnion
			 union)
		{
			return Db4objects.Db4o.Internal.Btree.Algebra.BTreeAlgebra.Union(_union, union);
		}

		protected override Db4objects.Db4o.Internal.Btree.IBTreeRange Execute(Db4objects.Db4o.Internal.Btree.BTreeRangeSingle
			 single)
		{
			return Db4objects.Db4o.Internal.Btree.Algebra.BTreeAlgebra.Union(_union, single);
		}
	}
}
