namespace Db4objects.Db4o.Inside.Btree
{
	/// <exclude></exclude>
	public interface IBTreeRangeVisitor
	{
		void Visit(Db4objects.Db4o.Inside.Btree.BTreeRangeSingle range);

		void Visit(Db4objects.Db4o.Inside.Btree.BTreeRangeUnion union);
	}
}
