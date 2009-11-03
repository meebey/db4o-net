/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class SharedIndexedFields
	{
		internal readonly VersionFieldMetadata _version = new VersionFieldMetadata();

		internal readonly UUIDFieldMetadata _uUID = new UUIDFieldMetadata();
	}
}
