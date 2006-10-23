namespace Db4objects.Db4o.Inside.Btree.Algebra
{
	/// <exclude></exclude>
	public abstract class BTreeRangeUnionOperation : Db4objects.Db4o.Inside.Btree.Algebra.BTreeRangeOperation
	{
		protected readonly Db4objects.Db4o.Inside.Btree.BTreeRangeUnion _union;

		public BTreeRangeUnionOperation(Db4objects.Db4o.Inside.Btree.BTreeRangeUnion union
			)
		{
			_union = union;
		}
	}
}
