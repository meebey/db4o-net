namespace Db4objects.Db4o.Internal.Btree
{
	/// <exclude></exclude>
	public class BTreeRemove : Db4objects.Db4o.Internal.Btree.BTreeUpdate
	{
		public BTreeRemove(Db4objects.Db4o.Internal.Transaction transaction, object obj) : 
			base(transaction, obj)
		{
		}

		protected override void Committed(Db4objects.Db4o.Internal.Btree.BTree btree)
		{
			btree.NotifyRemoveListener(GetObject());
		}

		public override string ToString()
		{
			return "(-) " + base.ToString();
		}

		public override bool IsRemove()
		{
			return true;
		}

		protected override object GetCommittedObject()
		{
			return Db4objects.Db4o.Foundation.No4.INSTANCE;
		}
	}
}
