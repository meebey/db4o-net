using Db4objects.Db4o.Internal.Btree;

namespace Db4objects.Db4o.Internal.Btree
{
	/// <exclude></exclude>
	public interface IBTreeRangeVisitor
	{
		void Visit(BTreeRangeSingle range);

		void Visit(BTreeRangeUnion union);
	}
}
