namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class LazyObjectReference : Db4objects.Db4o.Ext.IObjectInfo
	{
		private readonly Db4objects.Db4o.Internal.Transaction _transaction;

		private readonly int _id;

		public LazyObjectReference(Db4objects.Db4o.Internal.Transaction transaction, int 
			id)
		{
			_transaction = transaction;
			_id = id;
		}

		public virtual long GetInternalID()
		{
			return _id;
		}

		public virtual object GetObject()
		{
			return Reference().GetObject();
		}

		public virtual Db4objects.Db4o.Ext.Db4oUUID GetUUID()
		{
			return Reference().GetUUID();
		}

		public virtual long GetVersion()
		{
			return Reference().GetVersion();
		}

		private Db4objects.Db4o.Internal.ObjectReference Reference()
		{
			Db4objects.Db4o.Internal.HardObjectReference hardRef = _transaction.Stream().GetHardObjectReferenceById
				(_transaction, _id);
			return hardRef._reference;
		}
	}
}
