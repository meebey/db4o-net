using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class LazyObjectReference : IObjectInfo
	{
		private readonly Transaction _transaction;

		private readonly int _id;

		public LazyObjectReference(Transaction transaction, int id)
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

		public virtual Db4oUUID GetUUID()
		{
			return Reference().GetUUID();
		}

		public virtual long GetVersion()
		{
			return Reference().GetVersion();
		}

		private ObjectReference Reference()
		{
			HardObjectReference hardRef = _transaction.Stream().GetHardObjectReferenceById(_transaction
				, _id);
			return hardRef._reference;
		}
	}
}
