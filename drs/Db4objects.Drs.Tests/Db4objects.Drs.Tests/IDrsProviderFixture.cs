/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Drs.Inside;

namespace Db4objects.Drs.Tests
{
	public interface IDrsProviderFixture
	{
		ITestableReplicationProviderInside Provider();

		void Open();

		void Close();

		void Clean();

		void Destroy();
	}
}
