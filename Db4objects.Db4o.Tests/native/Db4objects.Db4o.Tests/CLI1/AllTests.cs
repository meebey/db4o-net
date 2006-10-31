using System;

namespace Db4objects.Db4o.Tests.CLI1
{
	public class AllTests : Db4oUnit.Extensions.Db4oTestSuite
	{
		protected override Type[] TestCases()
		{
			return new System.Type[]
				{   
					typeof(Inside.AllTests),
					typeof(MDArrayTestCase),
					typeof(CsCollections),
					typeof(CsStructs),
					typeof(CsStructsRegression),
				    typeof(CsValueTypesTestCase),
					typeof(ObjectSetAsListTestCase),
				};
		}
	}
}
