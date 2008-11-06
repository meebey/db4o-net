using System;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI2
{
	class AllTests : Db4oTestSuite
	{
		protected override Type[] TestCases()
		{
			return new Type[]
			{
				typeof(Assorted.AllTests),
				typeof(Collections.AllTests),
                typeof(Handlers.AllTests),
				typeof(NativeQueries.Diagnostics.AllTests),
				typeof(Regression.AllTests),
				typeof(Reflect.AllTests),
				typeof(TA.AllTests),
				typeof(Types.AllTests),
			};
		}
	}
}
