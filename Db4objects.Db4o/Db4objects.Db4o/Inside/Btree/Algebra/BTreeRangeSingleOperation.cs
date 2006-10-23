namespace Db4objects.Db4o.Inside.Btree.Algebra
{
	/// <exclude></exclude>
	public abstract class BTreeRangeSingleOperation : Db4objects.Db4o.Inside.Btree.Algebra.BTreeRangeOperation
	{
		protected readonly Db4objects.Db4o.Inside.Btree.BTreeRangeSingle _single;

		public BTreeRangeSingleOperation(Db4objects.Db4o.Inside.Btree.BTreeRangeSingle single
			)
		{
			_single = single;
		}
	}
}
