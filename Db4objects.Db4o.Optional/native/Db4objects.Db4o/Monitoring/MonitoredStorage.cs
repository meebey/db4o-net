/* Copyright (C) 2009   Versant Inc.   http://www.db4o.com */

#if !CF && !SILVERLIGHT

using System.Diagnostics;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Monitoring.Internal;

namespace Db4objects.Db4o.Monitoring
{
    /// <summary>
    /// Publishes performance counters for bytes read and written.
    /// </summary>
    public class MonitoredStorage : StorageDecorator
    {
        public MonitoredStorage(IStorage storage) : base(storage)
        {   
        }

        protected override IBin Decorate(BinConfiguration config, IBin bin)
        {
            return new MonitoredBin(bin);
        }

        internal class MonitoredBin : BinDecorator
        {
            private readonly PerformanceCounter _bytesWrittenCounter;
            private readonly PerformanceCounter _bytesReadCounter;

            public MonitoredBin(IBin bin) : base(bin)
            {
                _bytesWrittenCounter = Db4oPerformanceCounterCategory.CounterForBytesWrittenPerSec();
                _bytesReadCounter = Db4oPerformanceCounterCategory.CounterForBytesReadPerSec();
            }

            public override void Write(long position, byte[] bytes, int bytesToWrite)
            {
                base.Write(position, bytes, bytesToWrite);
                _bytesWrittenCounter.IncrementBy(bytesToWrite);
            }

            public override int Read(long position, byte[] buffer, int bytesToRead)
            {   
                int bytesRead = base.Read(position, buffer, bytesToRead);
                _bytesReadCounter.IncrementBy(bytesRead);
                return bytesRead;
            }

            public override int SyncRead(long position, byte[] bytes, int bytesToRead)
            {
                int bytesRead = base.SyncRead(position, bytes, bytesToRead);
                _bytesReadCounter.IncrementBy(bytesRead);
                return bytesRead;
            }

            public override void Close()
            {
                base.Close();
                _bytesReadCounter.Dispose();
                _bytesWrittenCounter.Dispose();
            }
        }
    }
}
#endif