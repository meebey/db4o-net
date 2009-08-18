/* Copyright (C) 2009   Versant Inc.   http://www.db4o.com */

#if !CF && !SILVERLIGHT

using System;
using System.Diagnostics;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Diagnostic;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Config;
using Db4objects.Db4o.Internal.Query;
using Db4objects.Db4o.Linq;
using Db4objects.Db4o.Monitoring.Internal;

namespace Db4objects.Db4o.Monitoring
{
	public class NativeQueryMonitoringSupport : IConfigurationItem
	{
		public void Prepare(IConfiguration configuration)
		{
#if CF_3_5 || NET_3_5
			var common = Db4oLegacyConfigurationBridge.AsCommonConfiguration(configuration);
			common.Environment.Add(new LinqQueryMonitor());
#endif
		}

		public void Apply(IInternalObjectContainer container)
		{
			IEventRegistry eventRegistry = EventRegistryFactory.ForObjectContainer(container);
			
			PerformanceCounter unoptimizedNativeQueriesPerSec = Db4oPerformanceCounterCategory.CounterForUnoptimizedNativeQueriesPerSec(false);
			PerformanceCounter nativeQueriesPerSec = Db4oPerformanceCounterCategory.CounterForNativeQueriesPerSec(false);
			container.GetNativeQueryHandler().QueryExecution += delegate(object sender, QueryExecutionEventArgs args)
			{
				if (args.ExecutionKind == QueryExecutionKind.Unoptimized)
					unoptimizedNativeQueriesPerSec.Increment();

				nativeQueriesPerSec.Increment();
			};

			eventRegistry.Closing += delegate
			{
				nativeQueriesPerSec.Dispose();
				unoptimizedNativeQueriesPerSec.Dispose();
			
#if CF_3_5 || NET_3_5
				container.WithEnvironment(delegate
				{
					My<LinqQueryMonitor>.Instance.Dispose();
				});
#endif

			};
		}


#if CF_3_5 || NET_3_5
		class LinqQueryMonitor : ILinqQueryMonitor
		{
			private readonly PerformanceCounter _queriesPerSec =
				Db4oPerformanceCounterCategory.CounterForLinqQueriesPerSec(false);

			private readonly PerformanceCounter _unoptimizedQueriesPerSec =
				Db4oPerformanceCounterCategory.CounterForUnoptimizedLinqQueriesPerSec(false);

			public void OnOptimizedQuery()
			{
				_queriesPerSec.Increment();
			}

			public void OnUnoptimizedQuery()
			{
				_queriesPerSec.Increment();
				_unoptimizedQueriesPerSec.Increment();
			}

			public void Dispose()
			{
				_queriesPerSec.Dispose();
			}
		}
#endif
	}

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

			PerformanceCounter classIndexScansPerSec = Db4oPerformanceCounterCategory.CounterForClassIndexScansPerSec(false);
			container.Configure().Diagnostic().AddListener(new DiagnosticListener(classIndexScansPerSec));
			
			eventRegistry.Closing += delegate
			{
				queriesPerSec.Dispose();
				classIndexScansPerSec.Dispose();
			};
		}

		class DiagnosticListener : IDiagnosticListener
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
	}
}

#endif