/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class SharedIndexedFields
	{
		internal readonly VersionFieldMetadata _version;

		internal readonly UUIDFieldMetadata _uUID;

		public SharedIndexedFields(ObjectContainerBase stream)
		{
			_version = new VersionFieldMetadata(stream);
			_uUID = new UUIDFieldMetadata(stream);
		}
	}
}
