/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests
{
	public class Student : Db4objects.Drs.Tests.Person
	{
		private string _studentno;

		public Student(string name, int age) : base(name, age)
		{
		}

		public virtual void SetStudentNo(string studentno)
		{
			this._studentno = studentno;
		}

		public virtual string GetStudentNo()
		{
			return _studentno;
		}
	}
}
