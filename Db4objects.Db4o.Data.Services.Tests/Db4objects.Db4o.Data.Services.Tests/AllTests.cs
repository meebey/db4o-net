/* Copyright (C) 2007 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Data.Services.Tests
{
	public class AllTests : Db4oTestSuite
	{
		public static int Main(string[] args)
		{
			var res = new AllTests().RunSolo();
			return res;
		}

		protected override Type[] TestCases()
		{
			return new []{
				typeof(Db4oDataContextTestCase),
			};
		}
	}
}
