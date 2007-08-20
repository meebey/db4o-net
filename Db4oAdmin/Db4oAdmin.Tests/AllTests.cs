/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;
using Db4oUnit;

namespace Db4oAdmin.Tests
{
	class AllTests : ReflectionTestSuite
	{
		protected override Type[] TestCases()
		{
			return new Type[]
				{
                    typeof(Core.ByAttributeTestCase),
					typeof(Core.ILPatternTestCase),
					typeof(Core.CustomInstrumentationTestCase),
					typeof(NQ.CFNQRuntimeOptimizationTestCase),
					typeof(NQ.PredicateBuildTimeOptimizationTestCase),
					typeof(NQ.UnoptimizablePredicatesTestCase),
					typeof(TA.TAInstrumentationTestCase),
				};
		}
	}
}
