using System.Collections;
using Db4objects.Db4o;
using Db4objects.Db4o.TA;
using Db4objects.Db4o.TA.Internal;
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
		internal Activator _activator;

		public Project(string name)
		{
			_name = name;
		}

		public virtual void Bind(IObjectContainer container)
		{
			if (null != _activator)
			{
				_activator.AssertCompatible(container);
				return;
			}
			_activator = new Activator(container, this);
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
			for (IEnumerator iter = _workLog.GetEnumerator(); iter.MoveNext(); )
			{
				UnitOfWork item = (UnitOfWork)iter.Current;
				total += item.TimeSpent();
			}
			return total;
		}
	}
}
