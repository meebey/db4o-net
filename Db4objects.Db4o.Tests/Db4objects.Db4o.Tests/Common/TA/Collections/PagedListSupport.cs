/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Tests.Common.TA.Collections;

namespace Db4objects.Db4o.Tests.Common.TA.Collections
{
	/// <summary>Configures the support for paged collections.</summary>
	/// <remarks>Configures the support for paged collections.</remarks>
	public class PagedListSupport : IConfigurationItem
	{
		public virtual void Apply(IInternalObjectContainer db)
		{
			EventRegistry(db).Updating += new Db4objects.Db4o.Events.CancellableObjectEventHandler
				(new _IEventListener4_17(this).OnEvent);
		}

		private sealed class _IEventListener4_17
		{
			public _IEventListener4_17(PagedListSupport _enclosing)
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
		// Nothing to do...
	}
}
