/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;

namespace Sharpen.Util
{
    public class Date
    {
        public static long ToJavaMilliseconds(DateTime dateTimeNet)
        {
            return dateTimeNet.Ticks / RATIO - DIFFERENCE_IN_TICKS;
        }

        private static long DIFFERENCE_IN_TICKS = 62135604000000;
        private static long RATIO = 10000;

        private long javaMilliSeconds;

        public Date() : this(DateTime.Now)
        {
        }

        public Date(long javaMilliSeconds)
        {
            this.javaMilliSeconds = javaMilliSeconds;
        }

        public Date(DateTime dateTimeNet)
        {
            javaMilliSeconds = ToJavaMilliseconds(dateTimeNet);
        }

        public long GetJavaMilliseconds()
        {
            return javaMilliSeconds;
        }

        public long GetTicks()
        {
            return (javaMilliSeconds + DIFFERENCE_IN_TICKS) * RATIO;
        }

        public long GetTime()
        {
            return GetJavaMilliseconds();
        }

        public void SetTime(long javaMilliSeconds)
        {
            this.javaMilliSeconds = javaMilliSeconds;
        }
    }
}
