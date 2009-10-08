/* Copyright (C) 2009   Versant Inc.   http://www.db4o.com */

#if !CF && !SILVERLIGHT

using System.Diagnostics;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Diagnostic;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Config;
using Db4objects.Db4o.Internal.Query;
using Db4objects.Db4o.Monitoring.Internal;

#if CF_3_5 || NET_3_5

using Db4objects.Db4o.Linq;

#endif

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
#if CF_3_5 || NET_3_5
			My<LinqQueryMonitor>.Instance.Initialize();
#endif

			IEventRegistry eventRegistry = EventRegistryFactory.ForObjectContainer(container);
			
			PerformanceCounter unoptimizedNativeQueriesPerSec = Db4oPerformanceCounterCategory.CounterFor(PerformanceCounterSpec.UnoptimizedNativeQueriesPerSec, false);
			PerformanceCounter nativeQueriesPerSec = Db4oPerformanceCounterCategory.CounterFor(PerformanceCounterSpec.NativeQueriesPerSec, false);
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
		    private PerformanceCounter _queriesPerSec;
		    private PerformanceCounter _unoptimizedQueriesPerSec;

			public void OnOptimizedQuery()
			{
				QueriesPerSec().Increment();
			}

		    public void OnUnoptimizedQuery()
			{
				QueriesPerSec().Increment();
				UnoptimizedQueriesPerSec().Increment();
			}

		    public void Dispose()
			{
                if (null != _queriesPerSec) _queriesPerSec.Dispose();
                if (null != _unoptimizedQueriesPerSec) _unoptimizedQueriesPerSec.Dispose();
			}
            
            private PerformanceCounter QueriesPerSec()
            {
                return _queriesPerSec;
            }
            
            private PerformanceCounter UnoptimizedQueriesPerSec()
            {
                return _unoptimizedQueriesPerSec;
            }

			public void Initialize()
			{
				_queriesPerSec = Db4oPerformanceCounterCategory.CounterFor(PerformanceCounterSpec.LinqQueriesPerSec, false);
				_unoptimizedQueriesPerSec = Db4oPerformanceCounterCategory.CounterFor(PerformanceCounterSpec.UnoptimizedLinqQueriesPerSec, false);
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
		    PerformanceCounter queriesPerSec = null;
		    PerformanceCounter classIndexScansPerSec = null;

		    container.WithEnvironment(delegate
            {
		        queriesPerSec = Db4oPerformanceCounterCategory.CounterFor(PerformanceCounterSpec.QueriesPerSec, false);
                classIndexScansPerSec = Db4oPerformanceCounterCategory.CounterFor(PerformanceCounterSpec.ClassIndexScansPerSec, false);
            });

			IEventRegistry eventRegistry = EventRegistryFactory.ForObjectContainer(container);
			eventRegistry.QueryFinished += delegate
			{
				queriesPerSec.Increment();
			};
			
			container.Configure().Diagnostic().AddListener(new DiagnosticListener(classIndexScansPerSec));
			
			eventRegistry.Closing += delegate
			{
				queriesPerSec.Dispose();
				classIndexScansPerSec.Dispose();
			};
		}

		class DiagnosticListener : IDiagnosticListener
		{
			private readonly PerformanceCounter _classIndexScansPerSec;

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