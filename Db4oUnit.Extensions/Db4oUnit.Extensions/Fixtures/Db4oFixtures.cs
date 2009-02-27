/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit.Extensions.Fixtures;

namespace Db4oUnit.Extensions.Fixtures
{
	public class Db4oFixtures
	{
		public static IConfigurationSource ConfigSource(bool independentConfig)
		{
			IConfigurationSource configSource = new IndependentConfigurationSource();
			if (!independentConfig)
			{
				configSource = new CachingConfigurationSource(configSource);
			}
			return configSource;
		}

		public static Db4oClientServer NewEmbeddedCS(bool independentConfig)
		{
			return new Db4oClientServer(ConfigSource(independentConfig), true, "C/S EMBEDDED"
				);
		}

		public static Db4oClientServer NewNetworkingCS(bool independentConfig)
		{
			return new Db4oClientServer(ConfigSource(independentConfig), false, "C/S");
		}

		public static Db4oSolo NewSolo(bool independentConfig)
		{
			return new Db4oSolo(ConfigSource(independentConfig));
		}
	}
}
