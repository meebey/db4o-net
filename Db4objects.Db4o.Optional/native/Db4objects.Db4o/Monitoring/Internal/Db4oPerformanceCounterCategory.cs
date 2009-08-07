/* Copyright (C) 2009   Versant Inc.   http://www.db4o.com */

#if !CF && !SILVERLIGHT

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Db4objects.Db4o.Monitoring.Internal
{
    class Db4oPerformanceCounterCategory
    {
        private const string BytesWrittenPerSec = "bytes written/sec";
        private const string BytesReadPerSec = "bytes read/sec";
        private const string ObjectsStoredPerSec = "objects stored/sec";

        public static void Install()
        {
//            PerformanceCounterCategory.Delete("Db4o");

            if (PerformanceCounterCategory.Exists("Db4o"))
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
                                 };
            PerformanceCounterCategory.Create("Db4o", "Db4o Performance Counters",
                                                     PerformanceCounterCategoryType.SingleInstance,
                                                     collection);
        }

        public static PerformanceCounter CounterForBytesReadPerSec()
        {
            return NewDb4oCounter(BytesReadPerSec);
        }

        public static PerformanceCounter CounterForBytesWrittenPerSec()
        {
            return NewDb4oCounter(BytesWrittenPerSec);
        }

        public static PerformanceCounter CounterForObjectsStoredPerSec()
        {
            return NewDb4oCounter(ObjectsStoredPerSec);
        }
       
        private static PerformanceCounter NewDb4oCounter(string counterName)
        {
        	Install();
            return new PerformanceCounter("Db4o", counterName, false)
                       {
                           RawValue = 0
                       };
        }
    }
}

#endif