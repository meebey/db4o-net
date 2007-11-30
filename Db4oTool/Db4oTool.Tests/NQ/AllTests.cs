using System;
using Db4oUnit;

namespace Db4oTool.Tests.NQ
{
	class AllTests : ReflectionTestSuite
	{
		protected override Type[] TestCases()
		{
			return new Type[]
				{
					typeof(CFNQRuntimeOptimizationTestCase),
					typeof(DelegateBuildTimeOptimizationTestCase),
					typeof(PredicateBuildTimeOptimizationTestCase),
					typeof(UnoptimizablePredicatesTestCase),
				};
		}
	}
}
