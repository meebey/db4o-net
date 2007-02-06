namespace Db4objects.Db4o.Internal.Btree.Algebra
{
	/// <exclude></exclude>
	public abstract class BTreeRangeSingleOperation : Db4objects.Db4o.Internal.Btree.Algebra.BTreeRangeOperation
	{
		protected readonly Db4objects.Db4o.Internal.Btree.BTreeRangeSingle _single;

		public BTreeRangeSingleOperation(Db4objects.Db4o.Internal.Btree.BTreeRangeSingle 
			single)
		{
			_single = single;
		}
	}
}
