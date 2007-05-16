/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class MutableBoolean
	{
		private bool _value;

		public virtual bool Value()
		{
			return _value;
		}

		public virtual void Set(bool val)
		{
			_value = val;
		}
	}
}
