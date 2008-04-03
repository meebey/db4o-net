using System;
using Db4objects.Drs.Tests.Dotnet;

namespace Db4objects.Drs.Tests
{
	partial class Db4oTests
	{
		protected override Type[] SpecificTestCases()
		{
			return new Type[]
			{
				typeof(StructTestCase),
				typeof(Regression.DRS46TestCase),
			};
		}
	}
}
