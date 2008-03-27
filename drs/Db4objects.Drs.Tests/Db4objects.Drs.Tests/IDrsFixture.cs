/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests
{
	public interface IDrsFixture
	{
		Db4objects.Drs.Inside.ITestableReplicationProviderInside Provider();

		void Open();

		void Close();

		void Clean();
	}
}
