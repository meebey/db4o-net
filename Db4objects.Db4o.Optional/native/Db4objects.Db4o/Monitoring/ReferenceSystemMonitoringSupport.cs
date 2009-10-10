/* Copyright (C) 2007   Versant Inc.   http://www.db4o.com */
#if !CF && !SILVERLIGHT


using System;
using System.Diagnostics;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.References;
using Db4objects.Db4o.Monitoring.Internal;

namespace Db4objects.Db4o.Monitoring
{
    public class ReferenceSystemMonitoringSupport : IConfigurationItem
    {
        private class ReferenceSystemListener : IReferenceSystemListener
        {
            private PerformanceCounter _performanceCounter;

            public ReferenceSystemListener(PerformanceCounter counter)
            {
                _performanceCounter = counter;
            }


            public void NotifyReferenceCountChanged(int changedBy)
            {
                _performanceCounter.IncrementBy(changedBy);
            }
        }

        private class MonitoringSupportReferenceSystemFactory : IReferenceSystemFactory, IDeepClone
        {

            private PerformanceCounter _performanceCounter;

            public IReferenceSystem NewReferenceSystem(IInternalObjectContainer container)
            {
                if (_performanceCounter == null)
                {
                    _performanceCounter =
                        Db4oPerformanceCounterCategory.CounterFor(PerformanceCounterSpec.ObjectReferenceCount,
                                                                  container, false);
                    IEventRegistry eventRegistry = EventRegistryFactory.ForObjectContainer(container);
                    eventRegistry.Closing += delegate
                    {
                        _performanceCounter.Dispose();
                    };
                }
                return new MonitoringReferenceSystem(new ReferenceSystemListener(_performanceCounter));
            }

            public object DeepClone(object context)
            {
                MonitoringSupportReferenceSystemFactory factory = new MonitoringSupportReferenceSystemFactory();
                factory._performanceCounter = _performanceCounter;
                return factory;
            }
        }


        public void Prepare(IConfiguration configuration)
        {
            ((Config4Impl)configuration).ReferenceSystemFactory(new MonitoringSupportReferenceSystemFactory());
        }

        public void Apply(IInternalObjectContainer container)
        {

        }
    }
}

#endif
