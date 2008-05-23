/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Config;

namespace Db4oUnit.Extensions.Fixtures
{
	public class CachingConfigurationSource : IConfigurationSource
	{
		private readonly IConfigurationSource _source;

		private IConfiguration _cached;

		public CachingConfigurationSource(IConfigurationSource source)
		{
			_source = source;
		}

		public virtual IConfiguration Config()
		{
			if (_cached == null)
			{
				_cached = _source.Config();
			}
			return _cached;
		}

		public virtual void Reset()
		{
			_cached = null;
		}
	}
}
