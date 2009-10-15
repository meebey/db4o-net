/* Copyright (C) 2009   Versant Inc.   http://www.db4o.com */

#if !CF && !SILVERLIGHT

using System.Diagnostics;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Monitoring
{
	public class ObjectMonitoringSupport : IConfigurationItem
	{
		public void Prepare(IConfiguration configuration)
		{
		}

		public void Apply(IInternalObjectContainer container)
		{
		    PerformanceCounter storedObjectsPerSec =
		        Db4oPerformanceCounters.CounterFor(PerformanceCounterSpec.ObjectsStoredPerSec, false);
			IEventRegistry eventRegistry = EventRegistryFactory.ForObjectContainer(container);
			eventRegistry.Created += delegate
										{
											storedObjectsPerSec.Increment();
										};
			eventRegistry.Closing += delegate
										{
											storedObjectsPerSec.Dispose();
										};
		}
	}
}

#endif