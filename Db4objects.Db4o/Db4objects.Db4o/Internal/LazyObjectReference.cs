namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class LazyObjectReference : Db4objects.Db4o.Ext.IObjectInfo
	{
		private readonly Db4objects.Db4o.Internal.ObjectContainerBase _container;

		private readonly int _id;

		public LazyObjectReference(Db4objects.Db4o.Internal.ObjectContainerBase container
			, int id)
		{
			_container = container;
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
			Db4objects.Db4o.Internal.HardObjectReference hardReference = _container.GetHardObjectReferenceById
				(_id);
			if (hardReference == null)
			{
				return null;
			}
			return hardReference._reference;
		}
	}
}
