/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public interface IReadable
	{
		object Read(Db4objects.Db4o.Internal.Buffer a_reader);

		int MarshalledLength();
	}
}
