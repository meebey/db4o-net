using System;
using Db4objects.Drs.Tests.Dotnet;

namespace Db4objects.Drs.Tests
{
	partial class Db4oTests
	{
		private Type[] PlatformSpecificTestCases()
		{
			return new Type[]
			{	
				typeof(Regression.DelegateTestCase),
				typeof(Regression.GenericListTestSuite),
                typeof(Regression.GenericDictionaryTestCase),
			};
		}
	}
}
