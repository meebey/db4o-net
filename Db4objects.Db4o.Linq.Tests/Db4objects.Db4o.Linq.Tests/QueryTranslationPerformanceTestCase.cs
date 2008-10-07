/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */
using System.Diagnostics;
using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Linq.Tests
{
	class QueryTranslationPerformanceTestCase : AbstractDb4oTestCase
	{
#if CF_3_5
		private const int TIME_LIMIT_WITH_FIRST_TIME_OVERHEAD = 6000;
		private const int TIME_LIMIT_NO_INITIAL_OVERHEAD = 200;
#else
		// Enough time to translate queries.. probably actual time will be shorter.
		private const int TIME_LIMIT_WITH_FIRST_TIME_OVERHEAD = 1000;
		private const int TIME_LIMIT_NO_INITIAL_OVERHEAD = 80;
#endif

		// This test only guarantee that drastic performance degradation doens't pass unnoticed.
		public void Test()
		{
			for (int i = 0; i < 3; i++)
			{
				long translationTime = TimeToTranslateQuery();

				Assert.IsSmaller(i == 0 ? TIME_LIMIT_WITH_FIRST_TIME_OVERHEAD : TIME_LIMIT_NO_INITIAL_OVERHEAD, translationTime);
			}
		}

		private long TimeToTranslateQuery()
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			var result = from TestSubject candidate in Db()
			             where candidate.Name == "Acv"
			             select candidate;

			sw.Stop();
			return sw.ElapsedMilliseconds;
		}

		protected override void Store()
		{
			Store(new TestSubject("Acv"));
			Store(new TestSubject("Gcrav"));
		}

		class TestSubject
		{
			public TestSubject(string name)
			{
				_name = name;	
			}

			public string Name
			{
				get { return _name; }
			}

			private readonly string _name;
		}
	}
}
