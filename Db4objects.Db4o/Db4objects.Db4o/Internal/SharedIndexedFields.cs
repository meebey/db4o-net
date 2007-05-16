/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	internal class SharedIndexedFields
	{
		internal readonly VersionFieldMetadata i_fieldVersion;

		internal readonly UUIDFieldMetadata i_fieldUUID;

		internal SharedIndexedFields(ObjectContainerBase stream)
		{
			i_fieldVersion = new VersionFieldMetadata(stream);
			i_fieldUUID = new UUIDFieldMetadata(stream);
		}
	}
}
