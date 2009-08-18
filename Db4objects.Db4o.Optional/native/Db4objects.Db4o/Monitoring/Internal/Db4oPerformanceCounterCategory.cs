/* Copyright (C) 2009   Versant Inc.   http://www.db4o.com */

#if !CF && !SILVERLIGHT

using System;
using System.Diagnostics;

namespace Db4objects.Db4o.Monitoring.Internal
{
    public class Db4oPerformanceCounterCategory
    {
    	public static readonly string CategoryName = "Db4o";

    	private const string BytesWrittenPerSec = "bytes written/sec";
        private const string BytesReadPerSec = "bytes read/sec";
        private const string ObjectsStoredPerSec = "objects stored/sec";
    	private const string QueriesPerSec = "queries/sec";
    	private const string ClassIndexScansPerSec = "class index scans/sec";
    	private const string LinqQueriesPerSec = "linq queries/sec";
		private const string UnoptimizedLinqQueriesPerSec = "unoptimized linq queries/sec";
		private const string NativeQueriesPerSec = "native queries/sec";
		private const string UnoptimizedNativeQueriesPerSec = "unoptimized native queries/sec";

    	public static void Install()
        {
            if (CategoryExists())
                return;

            var collection = new CounterCreationDataCollection
                                 {
                                     new CounterCreationData(BytesWrittenPerSec,
                                                             "Bytes written/sec",
                                                             PerformanceCounterType.RateOfCountsPerSecond32),
                                     new CounterCreationData(BytesReadPerSec,
                                                             "Bytes read/sec",
                                                             PerformanceCounterType.RateOfCountsPerSecond32),
                                     new CounterCreationData(ObjectsStoredPerSec,
                                                             "Number of objects stored per second.",
                                                             PerformanceCounterType.RateOfCountsPerSecond32),

									 new CounterCreationData(QueriesPerSec,
                                                             "Number of queries executed per second.",
                                                             PerformanceCounterType.RateOfCountsPerSecond32),
									 new CounterCreationData(ClassIndexScansPerSec,
                                                             "Number of queries that could not use field indexes and had to fall back to class index scans per second.",
                                                             PerformanceCounterType.RateOfCountsPerSecond32),

									 new CounterCreationData(NativeQueriesPerSec,
                                                             "Number of native queries executed per second.",
                                                             PerformanceCounterType.RateOfCountsPerSecond32),

									 new CounterCreationData(UnoptimizedNativeQueriesPerSec,
                                                             "Number of unoptimized native queries executed per second.",
                                                             PerformanceCounterType.RateOfCountsPerSecond32),
#if NET_3_5
									 new CounterCreationData(LinqQueriesPerSec,
                                                             "Number of Linq queries executed per second.",
                                                             PerformanceCounterType.RateOfCountsPerSecond32),

									 new CounterCreationData(UnoptimizedLinqQueriesPerSec,
                                                             "Number of unoptimized Linq queries executed per second.",
                                                             PerformanceCounterType.RateOfCountsPerSecond32),
#endif
                                 };
            PerformanceCounterCategory.Create(CategoryName, "Db4o Performance Counters",
                                                     PerformanceCounterCategoryType.SingleInstance,
                                                     collection);
        }

		public static void ReInstall()
		{
			if (CategoryExists()) DeleteCategory();
			Install();
		}

    	private static bool CategoryExists()
    	{
    		return PerformanceCounterCategory.Exists(CategoryName);
    	}

    	private static void DeleteCategory()
    	{
    		PerformanceCounterCategory.Delete(CategoryName);
    	}

    	public static PerformanceCounter CounterForQueriesPerSec(bool readOnly)
		{
			return NewDb4oCounter(QueriesPerSec, readOnly);
		}

		public static PerformanceCounter CounterForClassIndexScansPerSec(bool readOnly)
		{
			return NewDb4oCounter(ClassIndexScansPerSec, readOnly);
		}

#if NET_3_5
		public static PerformanceCounter CounterForLinqQueriesPerSec(bool readOnly)
		{
			return NewDb4oCounter(LinqQueriesPerSec, readOnly);
		}

		public static PerformanceCounter CounterForUnoptimizedLinqQueriesPerSec(bool readOnly)
		{
			return NewDb4oCounter(UnoptimizedLinqQueriesPerSec, readOnly);
		}
#endif

		public static PerformanceCounter CounterForNativeQueriesPerSec(bool readOnly)
		{
			return NewDb4oCounter(LinqQueriesPerSec, readOnly);
		}

		public static PerformanceCounter CounterForUnoptimizedNativeQueriesPerSec(bool readOnly)
		{
			return NewDb4oCounter(UnoptimizedLinqQueriesPerSec, readOnly);
		}

        public static PerformanceCounter CounterForBytesReadPerSec()
        {
            return NewDb4oCounter(BytesReadPerSec, false);
        }

        public static PerformanceCounter CounterForBytesWrittenPerSec()
        {
            return NewDb4oCounter(BytesWrittenPerSec, false);
        }

        public static PerformanceCounter CounterForObjectsStoredPerSec()
        {
            return NewDb4oCounter(ObjectsStoredPerSec, false);
        }
       
        private static PerformanceCounter NewDb4oCounter(string counterName, bool readOnly)
        {
        	Install();

        	PerformanceCounter counter = new PerformanceCounter(CategoryName, counterName, readOnly);
			if (readOnly) return counter;

			counter.RawValue = 0;
        	return counter;
        }
    }
}

#endif