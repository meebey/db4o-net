/* Copyright (C) 2007   Versant Inc.   http://www.db4o.com */
#if !CF && !SILVERLIGHT
using System;
using System.Diagnostics;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Monitoring;
using Db4objects.Db4o.Monitoring.Internal;
using Db4objects.Db4o.Query;
using Db4oUnit.Extensions;
using Db4oUnit;
using Db4oUnit.Extensions.Fixtures;

namespace Db4objects.Db4o.Tests.Monitoring
{
	class QueryMonitoringSupportTestCase : AbstractDb4oTestCase, ICustomClientServerConfiguration
	{
		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			config.Add(new QueryMonitoringSupport());
		}

		public void ConfigureServer(IConfiguration config)
		{
			Configure(config);
		}

		public void ConfigureClient(IConfiguration config)
		{
		}

		protected override void Db4oSetupBeforeConfigure()
		{
			Db4oPerformanceCounterCategory.ReInstall();
		}

		public void TestQueriesPerSecond()
		{
			using (PerformanceCounter counter = Db4oPerformanceCounterCategory.CounterForQueriesPerSec(true))
			{
				Assert.AreEqual(0, counter.RawValue);

				ExecuteGetAllQuery();
				ExecuteClassOnlyQuery();
				ExecuteOptimizedNQ();
				ExecuteUnoptimizedNQ();

				Assert.AreEqual(4, counter.RawValue);
			}
		}

		public void TestClassIndexScansPerSecond()
		{
			using (PerformanceCounter counter = Db4oPerformanceCounterCategory.CounterForClassIndexScansPerSec(true))
			{
				Assert.AreEqual(0, counter.RawValue);

				for (int i = 0; i < 3; ++i)
				{
					ExecuteSodaClassIndexScan();
					Assert.AreEqual(i + 1, counter.RawValue);
				}
			}
		}

		private void ExecuteClassOnlyQuery()
		{
			NewQuery(typeof (Item)).Execute();
		}

		private void ExecuteGetAllQuery()
		{
			NewQuery().Execute();
		}

		private void ExecuteOptimizedNQ()
		{
			Db().Query(delegate(Item item) { return item.id == 42; });
		}

		private void ExecuteUnoptimizedNQ()
		{
			Db().Query(delegate(Item item) { return item.GetType() == typeof (Item); });
		}
		
		private void ExecuteSodaClassIndexScan()
		{
			IQuery query = NewQuery(typeof(Item));
			query.Descend("_id").Constrain(42);
			query.Execute();
		}

		public class Item
		{
			public int id;
		}
	}
}
#endif
