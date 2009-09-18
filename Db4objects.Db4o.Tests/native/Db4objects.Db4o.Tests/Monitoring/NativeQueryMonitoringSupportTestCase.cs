/* Copyright (C) 2007   Versant Inc.   http://www.db4o.com */
#if !CF && !SILVERLIGHT
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Monitoring;
using Db4objects.Db4o.Monitoring.Internal;
using System.Diagnostics;
using Db4oUnit;

namespace Db4objects.Db4o.Tests.Monitoring
{
	class NativeQueryMonitoringSupportTestCase : QueryMonitoringSupportTestCaseBase
	{
		protected override void Configure(IConfiguration config)
		{
			config.Add(new NativeQueryMonitoringSupport());
		}
        
        public void TestNativeQueriesPerSecondPerformanceCount()
        {
			using (PerformanceCounter counter = Db4oPerformanceCounterCategory.CounterForNativeQueriesPerSec(MonitoredContainer()))
			{
				Assert.IsTrue(counter.CounterName.Contains("native queries")); 
			}
        }

		private IExtObjectContainer MonitoredContainer()
		{
			return IsEmbedded() 
				? FileSession() 
				: Db();
		}

		public void TestUnoptimizedNativeQueriesPerSecondPerformanceCount()
        {
			using (PerformanceCounter counter = Db4oPerformanceCounterCategory.CounterForUnoptimizedNativeQueriesPerSec(MonitoredContainer()))
			{
				Assert.IsTrue(counter.CounterName.Contains("native queries"));
			}
        }

		public void TestNativeQueriesPerSecondWithOptimizedQuery()
		{
			AssertCounter(
				Db4oPerformanceCounterCategory.CounterForNativeQueriesPerSec(MonitoredContainer()),
				ExecuteOptimizedNQ);
		}

		public void TestNativeQueriesPerSecondWithUnoptimizedQuery()
		{
			AssertCounter(
				Db4oPerformanceCounterCategory.CounterForNativeQueriesPerSec(MonitoredContainer()),
				ExecuteUnoptimizedNQ);
		}

		public void TestUnoptimizedNativeQueriesPerSecond()
		{
			AssertCounter(
				Db4oPerformanceCounterCategory.CounterForUnoptimizedNativeQueriesPerSec(MonitoredContainer()),
				ExecuteUnoptimizedNQ);
		}

#if CF_3_5 || NET_3_5
		public void TestLinqQueriesPerSecondWithOptimizedQuery()
		{
			AssertCounter(
				Db4oPerformanceCounterCategory.CounterForLinqQueriesPerSec(MonitoredContainer()),
				ExecuteOptimizedLinq);
		}

		public void TestLinqQueriesPerSecondWithUnoptimizedQuery()
		{
			AssertCounter(
				Db4oPerformanceCounterCategory.CounterForLinqQueriesPerSec(MonitoredContainer()),
				ExecuteUnoptimizedLinq);
		}

		public void TestUnoptimizedLinqQueriesPerSecond()
		{
			AssertCounter(
				Db4oPerformanceCounterCategory.CounterForUnoptimizedLinqQueriesPerSec(MonitoredContainer()),
				ExecuteUnoptimizedLinq);
		}

#endif

	}
}
#endif
