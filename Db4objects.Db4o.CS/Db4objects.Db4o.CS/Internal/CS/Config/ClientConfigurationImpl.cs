/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.CS.Config;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Config;
using Db4objects.Db4o.Messaging;

namespace Db4objects.Db4o.Internal.CS.Config
{
	public class ClientConfigurationImpl : Db4objects.Db4o.Internal.CS.Config.NetworkingConfigurationProviderImpl
		, IClientConfiguration
	{
		public ClientConfigurationImpl(Config4Impl config) : base(config)
		{
		}

		public virtual IMessageSender MessageSender
		{
			get
			{
				// TODO Auto-generated method stub
				throw new NotImplementedException();
			}
		}

		public virtual int PrefetchIDCount
		{
			set
			{
				int prefetchIDCount = value;
				// TODO Auto-generated method stub
				throw new NotImplementedException();
			}
		}

		public virtual int PrefetchObjectCount
		{
			set
			{
				int prefetchObjectCount = value;
				// TODO Auto-generated method stub
				throw new NotImplementedException();
			}
		}

		public virtual IBaseConfiguration Base
		{
			get
			{
				return new BaseConfigurationImpl(Legacy());
			}
		}
	}
}
