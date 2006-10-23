namespace Db4objects.Db4o.Inside.Btree
{
	/// <exclude></exclude>
	public class BTreeRemove : Db4objects.Db4o.Inside.Btree.BTreeUpdate
	{
		public BTreeRemove(Db4objects.Db4o.Transaction transaction, object obj) : base(transaction
			, obj)
		{
		}

		protected override void Committed(Db4objects.Db4o.Inside.Btree.BTree btree)
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
