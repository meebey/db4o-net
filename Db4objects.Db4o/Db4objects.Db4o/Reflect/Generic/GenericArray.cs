/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Reflect.Generic;

namespace Db4objects.Db4o.Reflect.Generic
{
	/// <exclude></exclude>
	public class GenericArray
	{
		internal GenericClass _clazz;

		internal object[] _data;

		public GenericArray(GenericClass clazz, int size)
		{
			_clazz = clazz;
			_data = new object[size];
		}

		internal virtual int GetLength()
		{
			return _data.Length;
		}
	}
}
