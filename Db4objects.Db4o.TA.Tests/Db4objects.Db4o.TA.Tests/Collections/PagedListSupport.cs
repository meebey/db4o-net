using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.TA.Tests.Collections.Internal;

namespace Db4objects.Db4o.TA.Tests.Collections
{
	/// <summary>Configures the support for paged collections.</summary>
	/// <remarks>Configures the support for paged collections.</remarks>
	public class PagedListSupport : IConfigurationItem
	{
		public virtual void Apply(ObjectContainerBase db)
		{
			EventRegistry(db).Updating += new Db4objects.Db4o.Events.CancellableObjectEventHandler
				(new _AnonymousInnerClass18(this).OnEvent);
		}

		private sealed class _AnonymousInnerClass18
		{
			public _AnonymousInnerClass18(PagedListSupport _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.CancellableObjectEventArgs
				 args)
			{
				CancellableObjectEventArgs cancellable = (CancellableObjectEventArgs)args;
				if (cancellable.Object is Page)
				{
					Page page = (Page)cancellable.Object;
					if (!page.IsDirty())
					{
						cancellable.Cancel();
					}
				}
			}

			private readonly PagedListSupport _enclosing;
		}

		private static IEventRegistry EventRegistry(IObjectContainer db)
		{
			return EventRegistryFactory.ForObjectContainer(db);
		}

		public virtual void Prepare(IConfiguration configuration)
		{
		}
	}
}
