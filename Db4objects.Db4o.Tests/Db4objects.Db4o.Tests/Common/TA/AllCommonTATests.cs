/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.TA;

namespace Db4objects.Db4o.Tests.Common.TA
{
	public class AllCommonTATests : Db4oTestSuite
	{
		public static void Main(string[] args)
		{
			new AllCommonTATests().RunAll();
		}

		protected override Type[] TestCases()
		{
			return new Type[] { typeof(Db4objects.Db4o.Tests.Common.TA.Events.AllTests), typeof(
				Db4objects.Db4o.Tests.Common.TA.Mixed.AllTests), typeof(Db4objects.Db4o.Tests.Common.TA.Nonta.AllTests
				), typeof(Db4objects.Db4o.Tests.Common.TA.Sample.AllTests), typeof(Db4objects.Db4o.Tests.Common.TA.TA.AllTests
				), typeof(ActivatableTestCase) };
		}
	}
}
