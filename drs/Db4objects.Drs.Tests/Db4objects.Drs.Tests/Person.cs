/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests
{
	public class Person
	{
		private string _name;

		private int _age;

		public virtual void SetName(string name)
		{
			this._name = name;
		}

		public virtual string GetName()
		{
			return _name;
		}

		public virtual void SetAge(int age)
		{
			this._age = age;
		}

		public virtual int GetAge()
		{
			return _age;
		}

		public Person(string name, int age)
		{
			this._name = name;
			this._age = age;
		}
	}
}
