/* Copyright (C) 2009   Versant Inc.   http://www.db4o.com */

#if !CF && !SILVERLIGHT

using System;
using System.Diagnostics;
using Db4objects.Db4o.CS.Internal;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Monitoring.Internal
{
    public class Db4oPerformanceCounterCategory
    {
    	public static readonly string CategoryName = "Db4o";

    	public static void Install()
        {
            if (CategoryExists())
            {
                return;
            }

    	    PerformanceCounterSpec[] performanceCounterSpecs = PerformanceCounterSpec.All();
    	    CounterCreationData[] creationData = new CounterCreationData[performanceCounterSpecs.Length];
            for(int i = 0; i < performanceCounterSpecs.Length; i++)
            {
                creationData[i] = performanceCounterSpecs[i].CounterCreationData();
            }

    	    CounterCreationDataCollection collection = new CounterCreationDataCollection(creationData);

            PerformanceCounterCategory.Create(CategoryName, "Db4o Performance Counters", PerformanceCounterCategoryType.MultiInstance, collection);
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

        public static PerformanceCounter CounterFor(PerformanceCounterSpec spec, bool readOnly)
        {
            return CounterFor(spec, My<IObjectContainer>.Instance, readOnly);
        }


        public static PerformanceCounter CounterFor(PerformanceCounterSpec spec, IObjectContainer container, bool readOnly)
        {
            return NewDb4oCounter(spec.Id, container.ToString(), readOnly);
        }


        /*
         * TODO: Remove 
         */
		public static PerformanceCounter CounterForNetworkingClientConnections(IObjectServer server)
		{
			PerformanceCounter clientConnections = NewDb4oCounter(PerformanceCounterSpec.NetClientConnections.Id, false);
			
			IObjectServerEvents serverEvents = (IObjectServerEvents) server;
			serverEvents.ClientConnected += delegate { clientConnections.Increment(); };
			serverEvents.ClientDisconnected += delegate { clientConnections.Decrement(); };

			return clientConnections;
		}

		private static PerformanceCounter NewDb4oCounter(string counterName, bool readOnly)
        {
        	string instanceName = My<IObjectContainer>.Instance.ToString();
        	return NewDb4oCounter(counterName, instanceName, readOnly);
        }

		private static PerformanceCounter NewDb4oCounter(string counterName, string instanceName)
		{
			return NewDb4oCounter(counterName, instanceName, true);
		}

    	private static PerformanceCounter NewDb4oCounter(string counterName, string instanceName, bool readOnly)
    	{
    		Install();

    		PerformanceCounter counter = new PerformanceCounter(CategoryName, counterName, instanceName, readOnly);

    		if (readOnly) return counter;

    		counter.RawValue = 0;
    		return counter;
    	}
    }
}

#endif