using System;
using Db4objects.Db4o.Tests.CLI2.Reflector;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI2.Reflect
{
	class AllTests : Db4oTestSuite
	{
		protected override Type[] TestCases()
		{
			return new Type[]
			       	{
			       		typeof(FastNetReflectorTestCase),
			       	};
		}
	}
}
