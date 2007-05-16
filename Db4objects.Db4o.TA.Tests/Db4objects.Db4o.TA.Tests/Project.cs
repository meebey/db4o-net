/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.TA;
using Db4objects.Db4o.TA.Tests;
using Db4objects.Db4o.TA.Tests.Collections;

namespace Db4objects.Db4o.TA.Tests
{
	internal class Project : IActivatable
	{
		internal IList _subProjects = new PagedList();

		internal IList _workLog = new PagedList();

		internal string _name;

		[System.NonSerialized]
		internal IActivator _activator;

		public Project(string name)
		{
			_name = name;
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

		public virtual void LogWorkDone(UnitOfWork work)
		{
			Activate();
			_workLog.Add(work);
		}

		public virtual long TotalTimeSpent()
		{
			Activate();
			long total = 0;
			IEnumerator i = _workLog.GetEnumerator();
			while (i.MoveNext())
			{
				UnitOfWork item = (UnitOfWork)i.Current;
				total += item.TimeSpent();
			}
			return total;
		}
	}
}
