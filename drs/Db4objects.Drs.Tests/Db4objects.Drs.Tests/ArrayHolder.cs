/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests
{
	public class ArrayHolder
	{
		public string _name;

		public Db4objects.Drs.Tests.ArrayHolder[] _array;

		public Db4objects.Drs.Tests.ArrayHolder[][] _arrayN;

		public ArrayHolder()
		{
		}

		public ArrayHolder(string name)
		{
			_name = name;
		}

		public override string ToString()
		{
			return _name + ", hashcode = " + GetHashCode();
		}
	}
}
