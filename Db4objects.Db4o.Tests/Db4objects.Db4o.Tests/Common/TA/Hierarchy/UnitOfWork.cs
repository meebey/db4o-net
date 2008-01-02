/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.Tests.Common.TA;

namespace Db4objects.Db4o.Tests.Common.TA.Hierarchy
{
	internal class UnitOfWork : ActivatableImpl
	{
		internal DateTime _started;

		internal DateTime _finished;

		internal string _name;

		public UnitOfWork(string name, DateTime started, DateTime finished)
		{
			_name = name;
			_started = started;
			_finished = finished;
		}

		public virtual string GetName()
		{
			Activate(ActivationPurpose.READ);
			return _name;
		}

		public virtual long TimeSpent()
		{
			Activate(ActivationPurpose.READ);
			return _finished.Ticks - _started.Ticks;
		}
	}
}
