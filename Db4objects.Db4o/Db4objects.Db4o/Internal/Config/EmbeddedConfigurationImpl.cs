/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Config;

namespace Db4objects.Db4o.Internal.Config
{
	public class EmbeddedConfigurationImpl : IEmbeddedConfiguration, ILegacyConfigurationProvider
	{
		private readonly Config4Impl _legacy;

		public EmbeddedConfigurationImpl(IConfiguration legacy)
		{
			_legacy = (Config4Impl)legacy;
		}

		public virtual ILocalConfiguration Local
		{
			get
			{
				return new LocalConfigurationImpl(_legacy);
			}
		}

		public virtual IBaseConfiguration Base
		{
			get
			{
				return new BaseConfigurationImpl(_legacy);
			}
		}

		public virtual Config4Impl Legacy()
		{
			return _legacy;
		}
	}
}
