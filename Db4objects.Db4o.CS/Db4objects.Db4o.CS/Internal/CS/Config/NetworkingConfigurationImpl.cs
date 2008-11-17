/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Config;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Messaging;

namespace Db4objects.Db4o.Internal.CS.Config
{
	public class NetworkingConfigurationImpl : INetworkingConfiguration
	{
		protected readonly Config4Impl _config;

		internal NetworkingConfigurationImpl(Config4Impl config)
		{
			_config = config;
		}

		public virtual Config4Impl Config()
		{
			return _config;
		}

		public virtual bool BatchMessages
		{
			set
			{
				bool flag = value;
				_config.BatchMessages(flag);
			}
		}

		public virtual int MaxBatchQueueSize
		{
			set
			{
				int maxSize = value;
				_config.MaxBatchQueueSize(maxSize);
			}
		}

		public virtual bool SingleThreadedClient
		{
			set
			{
				bool flag = value;
				_config.SingleThreadedClient(flag);
			}
		}

		public virtual int TimeoutClientSocket
		{
			set
			{
				int milliseconds = value;
				_config.TimeoutClientSocket(milliseconds);
			}
		}

		public virtual int TimeoutServerSocket
		{
			set
			{
				int milliseconds = value;
				_config.TimeoutServerSocket(milliseconds);
			}
		}

		public virtual IMessageRecipient MessageRecipient
		{
			set
			{
				IMessageRecipient messageRecipient = value;
				_config.SetMessageRecipient(messageRecipient);
			}
		}
	}
}
