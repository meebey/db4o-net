/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.CS.Config;
using Db4objects.Db4o.Internal;

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
				// TODO Auto-generated method stub
				throw new NotImplementedException();
			}
		}

		public virtual int MaxBatchQueueSize
		{
			set
			{
				int maxSize = value;
				// TODO Auto-generated method stub
				throw new NotImplementedException();
			}
		}

		public virtual bool SingleThreadedClient
		{
			set
			{
				bool flag = value;
				// TODO Auto-generated method stub
				throw new NotImplementedException();
			}
		}

		public virtual int TimeoutClientSocket
		{
			set
			{
				int milliseconds = value;
				// TODO Auto-generated method stub
				throw new NotImplementedException();
			}
		}

		public virtual int TimeoutServerSocket
		{
			set
			{
				int milliseconds = value;
				// TODO Auto-generated method stub
				throw new NotImplementedException();
			}
		}
	}
}
