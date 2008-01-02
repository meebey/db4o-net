/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Internal.Handlers
{
	internal interface INetType
	{
		object DefaultValue();

		int TypeID();

		void Write(object obj, byte[] bytes, int offset);

		object Read(byte[] bytes, int offset);
	}
}
