/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.CS.Config;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Config;

namespace Db4objects.Db4o.CS
{
	public class Db4oClientServer
	{
		public static IServerConfiguration NewServerConfiguration()
		{
			return new ServerConfigurationImpl(NewLegacyConfig());
		}

		public static IObjectServer OpenServer(IServerConfiguration config, string databaseFileName
			, int port)
		{
			Config4Impl legacy = LegacyFrom(config);
			return legacy.ClientServerFactory().OpenServer(legacy, databaseFileName, port, new 
				PlainSocketFactory());
		}

		public static IObjectContainer OpenClient(IClientConfiguration config, string host
			, int port, string user, string password)
		{
			Config4Impl legacy = LegacyFrom(config);
			return legacy.ClientServerFactory().OpenClient(legacy, host, port, user, password
				, new PlainSocketFactory());
		}

		private static Config4Impl LegacyFrom(INetworkingConfigurationProvider config)
		{
			return ((NetworkingConfigurationImpl)config.Networking).Config();
		}

		public static IClientConfiguration NewClientConfiguration()
		{
			return new ClientConfigurationImpl(NewLegacyConfig());
		}

		private static Config4Impl NewLegacyConfig()
		{
			return (Config4Impl)Db4oEmbedded.NewConfiguration();
		}
	}
}
