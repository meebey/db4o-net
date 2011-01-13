/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Internal.Btree
{
	public interface IFieldIndexKey
	{
		int ParentID();

		object Value();
	}
}
