namespace Db4objects.Db4o.Internal.Btree
{
	/// <exclude></exclude>
	public interface IBTreeRangeVisitor
	{
		void Visit(Db4objects.Db4o.Internal.Btree.BTreeRangeSingle range);

		void Visit(Db4objects.Db4o.Internal.Btree.BTreeRangeUnion union);
	}
}
