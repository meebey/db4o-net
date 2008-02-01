/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public interface IPersistent
	{
		byte GetIdentifier();

		int OwnLength();

		void ReadThis(Transaction trans, ByteArrayBuffer reader);

		void WriteThis(Transaction trans, ByteArrayBuffer writer);
	}
}
