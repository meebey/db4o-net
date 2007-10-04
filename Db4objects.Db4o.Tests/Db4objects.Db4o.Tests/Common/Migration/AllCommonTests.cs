/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Migration;
using Sharpen;

namespace Db4objects.Db4o.Tests.Common.Migration
{
	public class AllCommonTests : Db4oTestSuite
	{
		public static void Main(string[] args)
		{
			System.Environment.Exit(new AllCommonTests().RunSolo());
		}

		protected override Type[] TestCases()
		{
			return new Type[] { typeof(Db4oMigrationTestSuite) };
		}
	}
}
