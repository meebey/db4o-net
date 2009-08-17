/* Copyright (C) 2009   Versant Inc.   http://www.db4o.com */

#if !CF && !SILVERLIGHT

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

    	public static void Install()
        {
            if (PerformanceCounterCategory.Exists(CategoryName))
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
                                 };
            PerformanceCounterCategory.Create(CategoryName, "Db4o Performance Counters",
                                                     PerformanceCounterCategoryType.SingleInstance,
                                                     collection);
        }

		public static PerformanceCounter CounterForQueriesPerSec(bool readOnly)
		{
			return NewDb4oCounter(QueriesPerSec, readOnly);
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