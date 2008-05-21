/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Drs.Inside;

namespace Db4objects.Drs.Tests
{
	public interface IDrsFixture
	{
		ITestableReplicationProviderInside Provider();

		void Open();

		void Close();

		void Clean();
	}
}
