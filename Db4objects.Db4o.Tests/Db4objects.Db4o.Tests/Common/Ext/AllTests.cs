/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Ext;

namespace Db4objects.Db4o.Tests.Common.Ext
{
	public class AllTests : Db4oTestSuite
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Ext.AllTests().RunAll();
		}

		protected override Type[] TestCases()
		{
			return new Type[] { typeof(Db4oDatabaseTestCase), typeof(RefreshTestCase), typeof(
				StoredClassTestCase), typeof(StoredClassInstanceCountTestCase) };
		}
	}
}
