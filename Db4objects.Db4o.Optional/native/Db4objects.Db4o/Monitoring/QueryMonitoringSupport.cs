/* Copyright (C) 2009   Versant Inc.   http://www.db4o.com */

#if !CF && !SILVERLIGHT

using System;
using System.Diagnostics;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Diagnostic;
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

		public class DiagnosticListener : IDiagnosticListener
		{
			private PerformanceCounter _classIndexScansPerSec;

			public DiagnosticListener(PerformanceCounter classIndexScansPerSec)
			{
				_classIndexScansPerSec = classIndexScansPerSec;
			}

			public void OnDiagnostic(IDiagnostic d)
			{
				LoadedFromClassIndex classIndexScan = d as LoadedFromClassIndex;
				if (classIndexScan == null)
					return;

				_classIndexScansPerSec.Increment();
			}
		}

		public void Apply(IInternalObjectContainer container)
		{
			PerformanceCounter queriesPerSec = Db4oPerformanceCounterCategory.CounterForQueriesPerSec(false);
			IEventRegistry eventRegistry = EventRegistryFactory.ForObjectContainer(container);
			eventRegistry.QueryFinished += delegate
			{
				queriesPerSec.Increment();
			};

			PerformanceCounter classIndexScansPerSec = Db4oPerformanceCounterCategory.CounterForClassIndexScansPerSec(false);
			container.Configure().Diagnostic().AddListener(new DiagnosticListener(classIndexScansPerSec));

			eventRegistry.Closing += delegate
			{
				queriesPerSec.Dispose();
				classIndexScansPerSec.Dispose();
			};
		}
	}
}

#endif