/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Staging;

namespace Db4objects.Db4o.Tests.Common.Staging
{
	public class AllTests : Db4oTestSuite
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Staging.AllTests().RunSolo();
		}

		protected override Type[] TestCases()
		{
			return new Type[] { typeof(ActivateDepthTestCase), typeof(ClientServerPingTestCase
				), typeof(InterfaceQueryTestCase), typeof(LazyQueryDeleteTestCase), typeof(PingTestCase
				), typeof(SODAClassTypeDescend) };
		}
		// TODO is there a Jira for this one?
	}
}
