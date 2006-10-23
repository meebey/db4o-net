namespace Db4objects.Db4o.Inside.Btree
{
	/// <exclude></exclude>
	public class BTreeCancelledRemoval : Db4objects.Db4o.Inside.Btree.BTreeUpdate
	{
		private readonly object _newKey;

		public BTreeCancelledRemoval(Db4objects.Db4o.Transaction transaction, object originalKey
			, object newKey, Db4objects.Db4o.Inside.Btree.BTreeUpdate existingPatches) : base
			(transaction, originalKey)
		{
			_newKey = newKey;
			if (null != existingPatches)
			{
				Append(existingPatches);
			}
		}

		protected override void Committed(Db4objects.Db4o.Inside.Btree.BTree btree)
		{
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
