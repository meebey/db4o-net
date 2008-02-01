/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public interface IMarshallingInfo : IFieldListInfo
	{
		Db4objects.Db4o.Internal.ClassMetadata ClassMetadata();

		IReadWriteBuffer Buffer();
	}
}
