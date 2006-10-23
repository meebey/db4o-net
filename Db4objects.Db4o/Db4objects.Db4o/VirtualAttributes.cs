namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public class VirtualAttributes : Db4objects.Db4o.Foundation.IShallowClone
	{
		public Db4objects.Db4o.Ext.Db4oDatabase i_database;

		public long i_version;

		public long i_uuid;

		public virtual object ShallowClone()
		{
			Db4objects.Db4o.VirtualAttributes va = new Db4objects.Db4o.VirtualAttributes();
			va.i_database = i_database;
			va.i_version = i_version;
			va.i_uuid = i_uuid;
			return va;
		}

		internal virtual bool SuppliesUUID()
		{
			return i_database != null && i_uuid != 0;
		}
	}
}
