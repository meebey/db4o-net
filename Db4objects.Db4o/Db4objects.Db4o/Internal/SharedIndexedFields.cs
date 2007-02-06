namespace Db4objects.Db4o.Internal
{
	internal class SharedIndexedFields
	{
		internal readonly Db4objects.Db4o.Internal.VersionFieldMetadata i_fieldVersion;

		internal readonly Db4objects.Db4o.Internal.UUIDFieldMetadata i_fieldUUID;

		internal SharedIndexedFields(Db4objects.Db4o.Internal.ObjectContainerBase stream)
		{
			i_fieldVersion = new Db4objects.Db4o.Internal.VersionFieldMetadata(stream);
			i_fieldUUID = new Db4objects.Db4o.Internal.UUIDFieldMetadata(stream);
		}
	}
}
