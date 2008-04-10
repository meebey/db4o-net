using System;
using Db4oUnit;

namespace Db4oTool.Tests.TA
{
	class AllTests : ReflectionTestSuite
	{
		protected override Type[] TestCases()
		{
			return new Type[]
				{
					typeof(TABytecodeChangesTestCase),
					typeof(TAInstrumentationTestCase),
                    typeof(TAInstrumentationAppliedMoreThanOnce),
					typeof(TANonStorableTypeTestCase),
				};
		}
	}
}
