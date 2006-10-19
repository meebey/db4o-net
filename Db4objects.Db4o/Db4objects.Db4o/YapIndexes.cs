namespace Db4objects.Db4o
{
	internal class YapIndexes
	{
		internal readonly Db4objects.Db4o.YapFieldVersion i_fieldVersion;

		internal readonly Db4objects.Db4o.YapFieldUUID i_fieldUUID;

		internal YapIndexes(Db4objects.Db4o.YapStream stream)
		{
			i_fieldVersion = new Db4objects.Db4o.YapFieldVersion(stream);
			i_fieldUUID = new Db4objects.Db4o.YapFieldUUID(stream);
		}
	}
}
