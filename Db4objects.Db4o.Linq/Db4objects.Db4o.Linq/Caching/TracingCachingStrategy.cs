using System;
using System.Diagnostics;

namespace Db4objects.Db4o.Linq.Caching
{
	public class TracingCachingStrategy<TKey, TValue> : ICachingStrategy<TKey, TValue>
	{
		private readonly ICachingStrategy<TKey, TValue> _delegate;

		public TracingCachingStrategy(ICachingStrategy<TKey, TValue> @delegate)
		{
			_delegate = @delegate;
		}

		public TValue Produce(TKey key, Func<TKey, TValue> producer)
		{
			var hit = true;
			var result = _delegate.Produce(key, delegate(TKey newKey)
			                                    {
			                                    	hit = false;
			                                    	return producer(newKey);
			                                    });
			TraceCacheHitMiss(key, hit);
			return result;
		}

		private void TraceCacheHitMiss(TKey key, bool hit)
		{
			if (hit)
				Trace.WriteLine("Cache hit: " + key);
			else
				Trace.WriteLine("Cache miss: " + key);
		}
	}
}
