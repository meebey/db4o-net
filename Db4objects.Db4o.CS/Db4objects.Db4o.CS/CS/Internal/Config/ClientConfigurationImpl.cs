/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Config;
using Db4objects.Db4o.CS.Internal.Config;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Config;
using Db4objects.Db4o.Messaging;

namespace Db4objects.Db4o.CS.Internal.Config
{
	public class ClientConfigurationImpl : NetworkingConfigurationProviderImpl, IClientConfiguration
	{
		public ClientConfigurationImpl(Config4Impl config) : base(config)
		{
		}

		public virtual IMessageSender MessageSender
		{
			get
			{
				return Legacy().GetMessageSender();
			}
		}

		public virtual int PrefetchIDCount
		{
			set
			{
				int prefetchIDCount = value;
				Legacy().PrefetchIDCount(prefetchIDCount);
			}
		}

		public virtual int PrefetchObjectCount
		{
			set
			{
				int prefetchObjectCount = value;
				Legacy().PrefetchObjectCount(prefetchObjectCount);
			}
		}

		public virtual ICommonConfiguration Common
		{
			get
			{
				return Db4oLegacyConfigurationBridge.AsCommonConfiguration(Legacy());
			}
		}

		public virtual int PrefetchDepth
		{
			set
			{
				int prefetchDepth = value;
				Legacy().PrefetchDepth(prefetchDepth);
			}
		}

		public virtual int PrefetchSlotCacheSize
		{
			set
			{
				int slotCacheSize = value;
				Legacy().PrefetchSlotCacheSize(slotCacheSize);
			}
		}
	}
}
