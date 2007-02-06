namespace Db4objects.Db4o.Internal.Replication
{
	/// <exclude></exclude>
	public interface IDb4oReplicationReference
	{
		Db4objects.Db4o.Ext.Db4oDatabase SignaturePart();

		long LongPart();

		long Version();
	}
}
