/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;
using Db4oUnit;

namespace Db4oAdmin.Tests
{
	class AllTests : ReflectionTestSuite
	{
		protected override Type[] TestCases()
		{
//			return new Type[] { typeof(Core.PreserveDebugInfoTestCase), };
			return new Type[]
				{
					typeof(Core.ByNameTestCase),
					typeof(Core.ByAttributeTestCase),
					typeof(Core.ByFilterTestCase),
					typeof(Core.ByNotAttributeTestCase),
					typeof(Core.ILPatternTestCase),
					typeof(Core.CustomInstrumentationTestCase),
					typeof(Core.PreserveDebugInfoTestCase),
					typeof(NQ.CFNQRuntimeOptimizationTestCase),
					typeof(NQ.PredicateBuildTimeOptimizationTestCase),
					typeof(NQ.UnoptimizablePredicatesTestCase),
					typeof(TA.TAInstrumentationTestCase),
				};
		}
	}
}
