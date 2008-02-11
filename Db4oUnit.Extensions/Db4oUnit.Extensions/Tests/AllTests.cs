/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions.Tests;

namespace Db4oUnit.Extensions.Tests
{
	public class AllTests : ReflectionTestSuite
	{
		public static void Main(string[] args)
		{
			new AllTests().Run();
		}

		protected override Type[] TestCases()
		{
			return new Type[] { typeof(FixtureConfigurationTestCase), typeof(FixtureTestCase)
				 };
		}
	}
}
