using System;

namespace Db4objects.Db4o.Tests.CLI1
{
	public class AllTests : Db4oUnit.Extensions.Db4oTestSuite
	{
		protected override Type[] TestCases()
		{
			return new System.Type[]
				{
#if !CF_1_0 && !CF_2_0
					typeof(CsAppDomains),
					typeof(CsAssemblyVersionChange),
					typeof(CsImage),
#endif
					typeof(Events.EventRegistryTestCase),
					typeof(Inside.AllTests),
					typeof(NativeQueries.AllTests),
					typeof(CsCascadeDeleteToStructs),
					typeof(CsCollections),
					typeof(CsCustomTransientAttribute),
					typeof(CsDate),
					typeof(CsDelegate),
					typeof(CsDisposableTestCase),
					typeof(CsEnum),
					typeof(CsEvaluationDelegate),
					typeof(CsMarshalByRef),
					typeof(CsType),
					typeof(CsStructs),
					typeof(CsStructsRegression),
					typeof(CsValueTypesTestCase),
					typeof(CultureInfoTestCase),
					typeof(MDArrayTestCase),
					typeof(ObjectSetAsListTestCase),
				};
		}
	}
}
