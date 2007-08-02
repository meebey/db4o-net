/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.TA.Tests;
using Sharpen.Util;

namespace Db4objects.Db4o.TA.Tests
{
	internal class UnitOfWork : ActivatableImpl
	{
		internal Date _started;

		internal Date _finished;

		internal string _name;

		public UnitOfWork(string name, Date started, Date finished)
		{
			_name = name;
			_started = started;
			_finished = finished;
		}

		public virtual string GetName()
		{
			Activate();
			return _name;
		}

		public virtual long TimeSpent()
		{
			Activate();
			return _finished.GetTime() - _started.GetTime();
		}
	}
}
