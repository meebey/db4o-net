/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Config;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Config;

namespace Db4objects.Db4o.Internal.CS.Config
{
	public class NetworkingConfigurationProviderImpl : INetworkingConfigurationProvider
	{
		private readonly NetworkingConfigurationImpl _networking;

		public NetworkingConfigurationProviderImpl(Config4Impl config)
		{
			_networking = new NetworkingConfigurationImpl(config);
		}

		public virtual INetworkingConfiguration Networking
		{
			get
			{
				return _networking;
			}
		}

		protected virtual Config4Impl Config()
		{
			return _networking.Config();
		}
	}
}
