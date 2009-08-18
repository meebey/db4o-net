/* Copyright (C) 2007   Versant Inc.   http://www.db4o.com */
#if !CF && !SILVERLIGHT
using Db4objects.Db4o.Monitoring;
using Db4objects.Db4o.Monitoring.Internal;

namespace Db4objects.Db4o.Tests.Monitoring
{
	class NativeQueryMonitoringSupportTestCase : QueryMonitoringSupportTestCaseBase
	{
		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			config.Add(new NativeQueryMonitoringSupport());
		}

		public void TestNativeQueriesPerSecondWithOptimizedQuery()
		{
			AssertCounter(
				Db4oPerformanceCounterCategory.CounterForNativeQueriesPerSec(true),
				ExecuteOptimizedNQ);
		}

		public void TestNativeQueriesPerSecondWithUnoptimizedQuery()
		{
			AssertCounter(
				Db4oPerformanceCounterCategory.CounterForNativeQueriesPerSec(true),
				ExecuteUnoptimizedNQ);
		}

		public void TestUnoptimizedNativeQueriesPerSecond()
		{
			AssertCounter(
				Db4oPerformanceCounterCategory.CounterForUnoptimizedNativeQueriesPerSec(true),
				ExecuteUnoptimizedNQ);
		}

#if CF_3_5 || NET_3_5
		public void TestLinqQueriesPerSecondWithOptimizedQuery()
		{
			AssertCounter(
				Db4oPerformanceCounterCategory.CounterForLinqQueriesPerSec(true),
				ExecuteOptimizedLinq);
		}

		public void TestLinqQueriesPerSecondWithUnoptimizedQuery()
		{
			AssertCounter(
				Db4oPerformanceCounterCategory.CounterForLinqQueriesPerSec(true),
				ExecuteUnoptimizedLinq);
		}

		public void TestUnoptimizedLinqQueriesPerSecond()
		{
			AssertCounter(
				Db4oPerformanceCounterCategory.CounterForUnoptimizedLinqQueriesPerSec(true),
				ExecuteUnoptimizedLinq);
		}

#endif

	}
}
#endif
