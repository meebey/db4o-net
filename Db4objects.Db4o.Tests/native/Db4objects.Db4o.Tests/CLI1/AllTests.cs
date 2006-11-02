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
					typeof(NativeQueries.AllTests),
					typeof(CsAppDomains),
					typeof(CsAssemblyVersionChange),
					typeof(CsCascadeDeleteToStructs),
					typeof(CsCollections),
					typeof(CsCustomTransientAttribute),
					typeof(CsDate),
					typeof(CsDelegate),
					typeof(CsDisposableTestCase),
					typeof(CsEnum),
					typeof(CsEvaluationDelegate),
					typeof(CsImage),
					typeof(CsMarshalByRef),
					typeof(CsType),
					typeof(CsStructs),
					typeof(CsStructsRegression),
					typeof(CsValueTypesTestCase),
					typeof(MDArrayTestCase),
					typeof(ObjectSetAsListTestCase),
				};
		}
	}
}
