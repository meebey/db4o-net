using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.IX;

namespace Db4objects.Db4o.Internal.Freespace
{
	/// <exclude></exclude>
	public class FreespaceBTree : BTree
	{
		public FreespaceBTree(Transaction trans, int id, IIndexable4 keyHandler) : base(trans
			, id, keyHandler)
		{
		}

		protected override bool CanEnlistWithTransaction()
		{
			return false;
		}
	}
}
