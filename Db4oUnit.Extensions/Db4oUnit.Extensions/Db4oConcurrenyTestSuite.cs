/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Concurrency;

namespace Db4oUnit.Extensions
{
	public abstract class Db4oConcurrenyTestSuite : AbstractDb4oTestCase, ITestSuiteBuilder
	{
		public virtual TestSuite Build()
		{
			return new Db4oConcurrencyTestSuiteBuilder(Fixture(), TestCases()).Build();
		}

		protected abstract override Type[] TestCases();
	}
}
