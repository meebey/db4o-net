using System;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.TA;

namespace Db4objects.Db4o.TA
{
	public class TransparentActivationSupport : IConfigurationItem
	{
		public virtual void Apply(ObjectContainerBase container)
		{
			container.Configure().ActivationDepth(0);
			EventRegistryFactory.ForObjectContainer(container).Instantiated += new Db4objects.Db4o.Events.ObjectEventHandler
				(new _AnonymousInnerClass13(this, container).OnEvent);
		}

		private sealed class _AnonymousInnerClass13
		{
			public _AnonymousInnerClass13(TransparentActivationSupport _enclosing, ObjectContainerBase
				 container)
			{
				this._enclosing = _enclosing;
				this.container = container;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectEventArgs args)
			{
				ObjectEventArgs oea = (ObjectEventArgs)args;
				if (oea.Object is IActivatable)
				{
					((IActivatable)oea.Object).Bind(container);
				}
			}

			private readonly TransparentActivationSupport _enclosing;

			private readonly ObjectContainerBase container;
		}
	}
}
