/* Copyright (C) 2004-2007   db4objects Inc.   http://www.db4o.com */
using System;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI2.Types
{
	class AllTests : Db4oTestSuite
	{
		protected override Type[] TestCases()
		{
			return new Type[]
				{
					typeof(ArrayAsGenericListTestCase),
					typeof(ArrayAsListTestCase),
				};
		}
	}
}
