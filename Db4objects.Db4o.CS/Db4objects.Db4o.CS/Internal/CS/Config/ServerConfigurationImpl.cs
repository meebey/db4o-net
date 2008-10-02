/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.CS.Config;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Config;
using Db4objects.Db4o.Messaging;

namespace Db4objects.Db4o.Internal.CS.Config
{
	public class ServerConfigurationImpl : Db4objects.Db4o.Internal.CS.Config.NetworkingConfigurationProviderImpl
		, IServerConfiguration
	{
		public ServerConfigurationImpl(Config4Impl config) : base(config)
		{
		}

		public virtual IMessageRecipient MessageRecipient
		{
			set
			{
				IMessageRecipient messageRecipient = value;
				// TODO Auto-generated method stub
				throw new NotImplementedException();
			}
		}

		public virtual ILocalConfiguration Local
		{
			get
			{
				return new LocalConfigurationImpl(Config());
			}
		}

		public virtual IBaseConfiguration Base
		{
			get
			{
				return new BaseConfigurationImpl(Config());
			}
		}
	}
}
