/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class MutableInt
	{
		private int _value;

		public MutableInt()
		{
		}

		public MutableInt(int value)
		{
			_value = value;
		}

		public virtual void Add(int addVal)
		{
			_value += addVal;
		}

		public virtual void Increment()
		{
			_value++;
		}

		public virtual void Substract(int substractVal)
		{
			_value -= substractVal;
		}

		public virtual int Value()
		{
			return _value;
		}

		public virtual void Value(int val)
		{
			_value = val;
		}
	}
}
