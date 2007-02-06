namespace Db4objects.Db4o.Internal.Btree.Algebra
{
	/// <exclude></exclude>
	public abstract class BTreeRangeUnionOperation : Db4objects.Db4o.Internal.Btree.Algebra.BTreeRangeOperation
	{
		protected readonly Db4objects.Db4o.Internal.Btree.BTreeRangeUnion _union;

		public BTreeRangeUnionOperation(Db4objects.Db4o.Internal.Btree.BTreeRangeUnion union
			)
		{
			_union = union;
		}
	}
}
