/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Freespace;

namespace Db4objects.Db4o.Tests.Common.Freespace
{
	public class AllTests : Db4oTestSuite
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Freespace.AllTests().RunSolo();
		}

		protected override Type[] TestCases()
		{
			return new Type[] { typeof(BlockConfigurationFileSizeTestCase), typeof(FileSizeTestCase
				), typeof(FreespaceManagerDiscardLimitTestCase), typeof(FreespaceManagerReopenTestCase
				), typeof(FreespaceManagerTestCase), typeof(FreespaceMigrationTestCase) };
		}
	}
}
