/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Set;

namespace Db4objects.Db4o.Tests.Common.Set
{
	public class AllTests : Db4oTestSuite
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Set.AllTests().RunAll();
		}

		protected override Type[] TestCases()
		{
			return new Type[] { typeof(DeepSetClientServerTestCase), typeof(DeepSetTestCase) };
		}
	}
}
