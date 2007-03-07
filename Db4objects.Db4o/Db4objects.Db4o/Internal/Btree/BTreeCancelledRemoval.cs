namespace Db4objects.Db4o.Internal.Btree
{
	/// <exclude></exclude>
	public class BTreeCancelledRemoval : Db4objects.Db4o.Internal.Btree.BTreeUpdate
	{
		private readonly object _newKey;

		public BTreeCancelledRemoval(Db4objects.Db4o.Internal.Transaction transaction, object
			 originalKey, object newKey, Db4objects.Db4o.Internal.Btree.BTreeUpdate existingPatches
			) : base(transaction, originalKey)
		{
			_newKey = newKey;
			if (null != existingPatches)
			{
				Append(existingPatches);
			}
		}

		protected override void Committed(Db4objects.Db4o.Internal.Btree.BTree btree)
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
