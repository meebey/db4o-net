/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;

namespace Db4oUnit.Extensions.Fixtures
{
	public class GlobalConfigurationSource : IConfigurationSource
	{
		private readonly IConfiguration _config = Db4oFactory.NewConfiguration();

		public virtual IConfiguration Config()
		{
			return _config;
		}
	}
}
