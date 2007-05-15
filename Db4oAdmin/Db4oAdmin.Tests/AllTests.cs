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
					typeof(TAInstrumentationTestCase),
					typeof(ILPatternTestCase),
					typeof(CFNQRuntimeOptimizationTestCase),
					typeof(PredicateBuildTimeOptimizationTestCase),
					typeof(UnoptimizablePredicatesTestCase),
					typeof(CustomInstrumentationTestCase),
				};
		}
	}
}
