/* Copyright (C) 2009   Versant Inc.   http://www.db4o.com */

#if !CF && !SILVERLIGHT

using System.Diagnostics;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Monitoring.Internal;

namespace Db4objects.Db4o.Monitoring
{
	public class QueryMonitoringSupport : IConfigurationItem
	{
		public void Prepare(IConfiguration configuration)
		{
		}

		public void Apply(IInternalObjectContainer container)
		{
			PerformanceCounter queriesPerSec = Db4oPerformanceCounterCategory.CounterForQueriesPerSec(false);
			IEventRegistry eventRegistry = EventRegistryFactory.ForObjectContainer(container);
			eventRegistry.QueryFinished += delegate
			{
				queriesPerSec.Increment();
			};
			eventRegistry.Closing += delegate
         	{
         		queriesPerSec.Dispose();
         	};
		}
	}
}

#endif