/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.TA;
using Sharpen.Util;

namespace Db4objects.Db4o.TA.Tests
{
	internal class UnitOfWork : IActivatable
	{
		internal Date _started;

		internal Date _finished;

		internal string _name;

		[System.NonSerialized]
		internal IActivator _activator;

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

		public virtual void Bind(IActivator activator)
		{
			if (null != _activator)
			{
				throw new InvalidOperationException();
			}
			_activator = activator;
		}

		protected virtual void Activate()
		{
			if (_activator == null)
			{
				return;
			}
			_activator.Activate();
		}

		public virtual long TimeSpent()
		{
			Activate();
			return _finished.GetTime() - _started.GetTime();
		}
	}
}
