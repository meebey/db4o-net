namespace Db4objects.Db4o.Inside.Btree.Algebra
{
	/// <exclude></exclude>
	public class BTreeRangeUnionUnion : Db4objects.Db4o.Inside.Btree.Algebra.BTreeRangeUnionOperation
	{
		public BTreeRangeUnionUnion(Db4objects.Db4o.Inside.Btree.BTreeRangeUnion union) : 
			base(union)
		{
		}

		protected override Db4objects.Db4o.Inside.Btree.IBTreeRange Execute(Db4objects.Db4o.Inside.Btree.BTreeRangeUnion
			 union)
		{
			return Db4objects.Db4o.Inside.Btree.Algebra.BTreeAlgebra.Union(_union, union);
		}

		protected override Db4objects.Db4o.Inside.Btree.IBTreeRange Execute(Db4objects.Db4o.Inside.Btree.BTreeRangeSingle
			 single)
		{
			return Db4objects.Db4o.Inside.Btree.Algebra.BTreeAlgebra.Union(_union, single);
		}
	}
}
