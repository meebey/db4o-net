/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Reflect.Self
{
	public interface ISelfReflectable
	{
		object Self_get(string fieldName);

		void Self_set(string fieldName, object value);
	}
}
