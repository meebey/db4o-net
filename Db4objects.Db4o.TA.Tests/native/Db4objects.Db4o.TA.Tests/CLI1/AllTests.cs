/* Copyright (C) 2004-2007   db4objects Inc.   http://www.db4o.com */
using System;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.TA.Tests.CLI1
{
	class AllTests : Db4oTestSuite
	{
		protected override Type[] TestCases()
		{
			return new Type[]
				{
					typeof(ValueTypeActivationTestCase),
				};
		}
	}
}
