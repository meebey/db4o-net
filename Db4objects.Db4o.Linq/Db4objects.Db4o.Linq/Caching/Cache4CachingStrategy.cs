
using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.Caching;

namespace Db4objects.Db4o.Linq.Caching
{
	public class Cache4CachingStrategy<TKey, TValue> : ICachingStrategy<TKey, TValue>
	{
		private readonly ICache4 _cache4;

		public Cache4CachingStrategy(ICache4 cache4)
		{
			_cache4 = cache4;
		}

		public TValue Produce(TKey key, Func<TKey, TValue> producer)
		{
			return (TValue) _cache4.Produce(key, new Function4Func<TKey, TValue>(producer), null);
		}
	}

	internal class Function4Func<T, TResult> : IFunction4
	{
		private readonly Func<T, TResult> _func;

		public Function4Func(Func<T, TResult> func)
		{
			_func = func;
		}

		public object Apply(object arg)
		{
			return _func((T) arg);
		}
	}
}
