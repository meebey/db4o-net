using System;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI1.CrossPlatform
{
	class AllTests : Db4oTestSuite
	{
		private static void Main()
		{
			new AllTests().RunClientServer();
		}

		protected override Type[] TestCases()
		{
			return new Type[] {typeof(CrossplatformTestCase), };

		}
	}
}
