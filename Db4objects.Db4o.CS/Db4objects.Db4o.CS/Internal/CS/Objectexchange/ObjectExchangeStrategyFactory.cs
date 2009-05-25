/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.CS.Objectexchange;

namespace Db4objects.Db4o.Internal.CS.Objectexchange
{
	public class ObjectExchangeStrategyFactory
	{
		public static IObjectExchangeStrategy ForConfig(ObjectExchangeConfiguration config
			)
		{
			if (config.prefetchDepth > 0)
			{
				return new EagerObjectExchangeStrategy(config);
			}
			return DeferredObjectExchangeStrategy.Instance;
		}
	}
}
