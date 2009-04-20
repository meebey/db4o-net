/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Drs.Inside;
using Db4objects.Drs.Tests;

namespace Db4objects.Drs.Tests.Transients
{
	public class TransientFixture : IDrsFixture
	{
		private string _name;

		private ITestableReplicationProviderInside _provider;

		public TransientFixture(string name)
		{
			_name = name;
		}

		public virtual ITestableReplicationProviderInside Provider()
		{
			return _provider;
		}

		public virtual void Clean()
		{
		}

		//do nothing
		public virtual void Close()
		{
			_provider.Destroy();
		}

		public virtual void Open()
		{
			_provider = new TransientReplicationProvider(new byte[] { 65 }, _name);
		}
	}
}
