/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.CS.Config;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Config;

namespace Db4objects.Db4o.CS
{
	/// <summary>
	/// Factory class to open db4o servers and to connect db4o clients
	/// to them.
	/// </summary>
	/// <remarks>
	/// Factory class to open db4o servers and to connect db4o clients
	/// to them.
	/// <br /><br />
	/// <b>Note:<br />
	/// This class is made available in db4o-X.x-cs-java.jar / Db4objects.Db4o.CS.dll</b>
	/// </remarks>
	/// <since>7.5</since>
	public class Db4oClientServer
	{
		/// <summary>
		/// creates a new
		/// <see cref="Db4objects.Db4o.CS.Config.IServerConfiguration">Db4objects.Db4o.CS.Config.IServerConfiguration
		/// 	</see>
		/// </summary>
		public static IServerConfiguration NewServerConfiguration()
		{
			return new ServerConfigurationImpl(NewLegacyConfig());
		}

		/// <summary>
		/// opens a db4o server with the specified configuration on
		/// the specified database file and provides access through
		/// the specified port.
		/// </summary>
		/// <remarks>
		/// opens a db4o server with the specified configuration on
		/// the specified database file and provides access through
		/// the specified port.
		/// </remarks>
		public static IObjectServer OpenServer(IServerConfiguration config, string databaseFileName
			, int port)
		{
			Config4Impl legacy = LegacyFrom(config);
			return legacy.ClientServerFactory().OpenServer(legacy, databaseFileName, port, new 
				PlainSocketFactory());
		}

		/// <summary>opens a db4o client instance with the specified configuration.</summary>
		/// <remarks>opens a db4o client instance with the specified configuration.</remarks>
		/// <param name="config">the configuration to be used</param>
		/// <param name="host">the host name of the server that is to be connected to</param>
		/// <param name="port">the server port to connect to</param>
		/// <param name="user">the username for authentication</param>
		/// <param name="password">the password for authentication</param>
		/// <seealso cref="Db4objects.Db4o.CS.Db4oClientServer.OpenServer">Db4objects.Db4o.CS.Db4oClientServer.OpenServer
		/// 	</seealso>
		/// <seealso cref="Db4objects.Db4o.IObjectServer.GrantAccess">Db4objects.Db4o.IObjectServer.GrantAccess
		/// 	</seealso>
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

		/// <summary>
		/// creates a new
		/// <see cref="Db4objects.Db4o.CS.Config.IClientConfiguration">Db4objects.Db4o.CS.Config.IClientConfiguration
		/// 	</see>
		/// 
		/// </summary>
		public static IClientConfiguration NewClientConfiguration()
		{
			return new ClientConfigurationImpl(NewLegacyConfig());
		}

		private static Config4Impl NewLegacyConfig()
		{
			return (Config4Impl)Db4oFactory.NewConfiguration();
		}
	}
}
