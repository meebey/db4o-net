/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;

namespace Db4oUnit
{
	public class NullTestSuiteBuilder : ITestSuiteBuilder
	{
		private TestSuite _suite;

		public NullTestSuiteBuilder(TestSuite suite)
		{
			_suite = suite;
		}

		public virtual TestSuite Build()
		{
			return _suite;
		}
	}
}
