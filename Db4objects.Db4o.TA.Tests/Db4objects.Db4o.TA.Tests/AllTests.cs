/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.TA.Tests;

namespace Db4objects.Db4o.TA.Tests
{
	public class AllTests : Db4oTestSuite
	{
		protected override Type[] TestCases()
		{
			return new Type[] { typeof(TransparentActivationDiagnosticsTestCase), typeof(TransparentActivationSupportTestCase)
				, typeof(TransparentActivationTestCase) };
		}

		public static void Main(string[] args)
		{
			new Db4objects.Db4o.TA.Tests.AllTests().RunSolo();
		}
	}
}
