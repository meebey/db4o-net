using System;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI1.NativeQueries
{
	class AllTests : Db4oTestSuite
	{
		protected override Type[] TestCases()
		{
			return new Type[]
			{
				typeof(Cat),
				typeof(ListElementByIdentity),
				typeof(MultipleAssemblySupportTestCase),
				typeof(NativeQueriesTestCase),
				typeof(OptimizationFailuresTestCase),
				typeof(StringComparisonTestCase),
			};
		}
	}
}
