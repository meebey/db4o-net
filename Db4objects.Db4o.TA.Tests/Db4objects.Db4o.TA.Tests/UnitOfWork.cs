using Db4objects.Db4o;
using Db4objects.Db4o.TA;
using Db4objects.Db4o.TA.Internal;
using Sharpen.Util;

namespace Db4objects.Db4o.TA.Tests
{
	internal class UnitOfWork : IActivatable
	{
		internal Date _started;

		internal Date _finished;

		internal string _name;

		[System.NonSerialized]
		internal Activator _activator;

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

		public virtual long TimeSpent()
		{
			Activate();
			return _finished.GetTime() - _started.GetTime();
		}
	}
}
