namespace Db4objects.Db4o.Internal.Btree.Algebra
{
	/// <exclude></exclude>
	public class BTreeRangeSingleUnion : Db4objects.Db4o.Internal.Btree.Algebra.BTreeRangeSingleOperation
	{
		public BTreeRangeSingleUnion(Db4objects.Db4o.Internal.Btree.BTreeRangeSingle single
			) : base(single)
		{
		}

		protected override Db4objects.Db4o.Internal.Btree.IBTreeRange Execute(Db4objects.Db4o.Internal.Btree.BTreeRangeSingle
			 single)
		{
			return Db4objects.Db4o.Internal.Btree.Algebra.BTreeAlgebra.Union(_single, single);
		}

		protected override Db4objects.Db4o.Internal.Btree.IBTreeRange Execute(Db4objects.Db4o.Internal.Btree.BTreeRangeUnion
			 union)
		{
			return Db4objects.Db4o.Internal.Btree.Algebra.BTreeAlgebra.Union(union, _single);
		}
	}
}
