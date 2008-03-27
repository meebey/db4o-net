/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests
{
	public class Car
	{
		internal string _model;

		internal Db4objects.Drs.Tests.Pilot _pilot;

		public Car()
		{
		}

		public Car(string model)
		{
			_model = model;
		}

		public virtual string GetModel()
		{
			return _model;
		}

		public virtual void SetModel(string model)
		{
			_model = model;
		}
	}
}
