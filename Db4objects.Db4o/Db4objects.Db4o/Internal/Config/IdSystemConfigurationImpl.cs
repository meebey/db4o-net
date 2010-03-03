/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal.Config
{
	/// <exclude></exclude>
	public class IdSystemConfigurationImpl : IIdSystemConfiguration
	{
		private readonly Config4Impl _config;

		public IdSystemConfigurationImpl(Config4Impl config)
		{
			_config = config;
		}

		public virtual void UsePointerBasedSystem()
		{
			_config.UsePointerBasedIdSystem();
		}

		public virtual void UseBTreeSystem()
		{
			_config.UseBTreeIdSystem();
		}

		public virtual void UseInMemorySystem()
		{
			_config.UseInMemoryIdSystem();
		}

		public virtual void UseCustomSystem(IIdSystemFactory factory)
		{
			_config.UseCustomIdSystem(factory);
		}
	}
}
