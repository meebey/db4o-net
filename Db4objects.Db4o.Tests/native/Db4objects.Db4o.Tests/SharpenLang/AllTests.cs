using System;

namespace Db4objects.Db4o.Tests.SharpenLang
{
	public class AllTests : Db4oUnit.Extensions.Db4oTestSuite
	{
		protected override Type[] TestCases()
		{
			return new System.Type[]
				{
                    typeof(TypeReferenceTestCase),
				};
		}
	}
}
