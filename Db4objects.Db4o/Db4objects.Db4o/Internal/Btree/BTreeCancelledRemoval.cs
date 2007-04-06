using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;

namespace Db4objects.Db4o.Internal.Btree
{
	/// <exclude></exclude>
	public class BTreeCancelledRemoval : BTreeUpdate
	{
		private readonly object _newKey;

		public BTreeCancelledRemoval(Transaction transaction, object originalKey, object 
			newKey, BTreeUpdate existingPatches) : base(transaction, originalKey)
		{
			_newKey = newKey;
			if (null != existingPatches)
			{
				Append(existingPatches);
			}
		}

		protected override void Committed(BTree btree)
		{
		}

		public override bool IsCancelledRemoval()
		{
			return true;
		}

		public override string ToString()
		{
			return "(u) " + base.ToString();
		}

		protected override object GetCommittedObject()
		{
			return _newKey;
		}
	}
}
