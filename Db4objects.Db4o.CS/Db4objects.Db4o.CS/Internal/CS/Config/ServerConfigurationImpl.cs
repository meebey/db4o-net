/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Config;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Config;

namespace Db4objects.Db4o.Internal.CS.Config
{
	public class ServerConfigurationImpl : Db4objects.Db4o.Internal.CS.Config.NetworkingConfigurationProviderImpl
		, IServerConfiguration
	{
		public ServerConfigurationImpl(Config4Impl config) : base(config)
		{
		}

		public virtual IFileConfiguration File
		{
			get
			{
				return new FileConfigurationImpl(Legacy());
			}
		}

		public virtual ICommonConfiguration Common
		{
			get
			{
				return new CommonConfigurationImpl(Legacy());
			}
		}
	}
}
