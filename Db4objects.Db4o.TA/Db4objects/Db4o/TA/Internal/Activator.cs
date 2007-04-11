using System;
using Db4objects.Db4o;

namespace Db4objects.Db4o.TA.Internal
{
	public class Activator
	{
		private readonly IObjectContainer _container;

		private bool _isActivated;

		private readonly object _subject;

		public Activator(IObjectContainer container, object subject)
		{
			_container = container;
			_subject = subject;
		}

		public virtual void AssertCompatible(IObjectContainer container)
		{
			if (_container == container)
			{
				return;
			}
			throw new InvalidOperationException();
		}

		public virtual void Activate()
		{
			if (_container == null)
			{
				return;
			}
			if (_isActivated)
			{
				return;
			}
			_container.Activate(_subject, ActivationDepth());
			_isActivated = true;
		}

		public virtual int ActivationDepth()
		{
			return Math.Max(ConfiguredActivationDepth(), 1);
		}

		private int ConfiguredActivationDepth()
		{
			return _container.Ext().Configure().ObjectClass(_subject.GetType()).MinimumActivationDepth
				();
		}
	}
}
