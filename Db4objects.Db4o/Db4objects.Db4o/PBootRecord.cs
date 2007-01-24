namespace Db4objects.Db4o
{
	/// <summary>Old database boot record class.</summary>
	/// <remarks>
	/// Old database boot record class.
	/// This class was responsible for storing the last timestamp id,
	/// for holding a reference to the Db4oDatabase object of the
	/// ObjectContainer and for holding on to the UUID index.
	/// This class is no longer needed with the change to the new
	/// fileheader. It still has to stay here to be able to read
	/// old databases.
	/// </remarks>
	/// <exclude></exclude>
	/// <persistent></persistent>
	public class PBootRecord : Db4objects.Db4o.P1Object, Db4objects.Db4o.IDb4oTypeImpl
		, Db4objects.Db4o.IInternal4
	{
		public Db4objects.Db4o.Ext.Db4oDatabase i_db;

		public long i_versionGenerator;

		public Db4objects.Db4o.MetaIndex i_uuidMetaIndex;

		public override int ActivationDepth()
		{
			return int.MaxValue;
		}

		public virtual Db4objects.Db4o.MetaIndex GetUUIDMetaIndex()
		{
			return i_uuidMetaIndex;
		}

		public virtual void Write(Db4objects.Db4o.YapFile file)
		{
			Db4objects.Db4o.Inside.SystemData systemData = file.SystemData();
			i_versionGenerator = systemData.LastTimeStampID();
			i_db = systemData.Identity();
			file.ShowInternalClasses(true);
			Store(2);
			file.ShowInternalClasses(false);
		}
	}
}
