/* Copyright (C) 2007   Versant Inc.   http://www.db4o.com */
#if !CF && !SILVERLIGHT
using System;
using System.Diagnostics;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Monitoring;
using Db4objects.Db4o.Monitoring.Internal;
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

		public void TestQueriesPerSecond()
		{
			PerformanceCounter queriesPerSec = Db4oPerformanceCounterCategory.CounterForQueriesPerSec(true);
			Assert.AreEqual(0, queriesPerSec.NextValue());
			
			Stopwatch stopwatch = Stopwatch.StartNew();
			int numIterations = 50;
			for (int i = 0; i < numIterations; i++)
			{
				NewQuery().Execute();
				NewQuery(typeof (Item)).Execute();
				Db().Query(delegate(Item item) { return item.id == 42; });
			}
			stopwatch.Stop();

			double actualValue = queriesPerSec.NextValue();

			int queryCount = numIterations * 3;
			Assert.AreEqual(queryCount, queriesPerSec.RawValue);

			double expected = queryCount * 1000.0/(stopwatch.ElapsedMilliseconds + 1);
			Assert.InRange(actualValue, expected * .95, expected * 1.05);

		}

		public class Item
		{
			public int id;
		}
	}
}
#endif
