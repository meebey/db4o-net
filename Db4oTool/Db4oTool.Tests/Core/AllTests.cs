using System;
using Db4oUnit;

namespace Db4oTool.Tests.Core
{
	class AllTests : ReflectionTestSuite
	{
		protected override Type[] TestCases()
		{
			return new Type[]
				{
					typeof(Core.ByNameTestCase),
					typeof(Core.ByAttributeTestCase),
					typeof(Core.ByFilterTestCase),
					typeof(Core.ByNotAttributeTestCase),
					typeof(Core.ContextVariableTestCase),
					typeof(Core.ILPatternTestCase),
					typeof(Core.CustomInstrumentationTestCase),
					typeof(Core.PreserveDebugInfoTestCase),
					typeof(Core.InstrumentingCFAssemblyTestCase),
				};
		}
	}
}
