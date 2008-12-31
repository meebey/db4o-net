/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;

namespace Db4objects.Db4o.Tests.CLI1
{
	public class AllTests : Db4oUnit.Extensions.Db4oTestSuite
	{
		protected override Type[] TestCases()
		{
			return new System.Type[]
				{
                    typeof(Aliases.AllTests),
					typeof(CrossPlatform.AllTests),
#if !CF
					typeof(CsAppDomains),
					typeof(CsAssemblyVersionChange),
					typeof(CsImage),
					typeof(ShutdownMultipleContainer),
#endif
					typeof(Events.EventRegistryTestCase),
                    typeof(Handlers.AllTests),
					typeof(Inside.AllTests),
					typeof(NativeQueries.AllTests),
					typeof(Reflect.Net.AllTests),
                    typeof(CollectionBaseTestCase),
					typeof(CsCascadeDeleteToStructs),
					typeof(CsCollections),
					typeof(CsCustomTransientAttribute),
					typeof(CsDate),
					typeof(CsDelegate),
					typeof(CsDisposableTestCase),
					typeof(CsEnum),
					// typeof(CsEvaluationDelegate),  moved to Staging because it fails
					typeof(CsMarshalByRef),
					typeof(CsType),
					typeof(CsStructs),
					typeof(CsStructsRegression),
					typeof(CsValueTypesTestCase),
					typeof(CultureInfoTestCase),
                    typeof(DictionaryBaseTestCase),
					typeof(ImageTestCase),
					typeof(JavaDateCompatibilityTestCase),
					typeof(JavaUUIDCompatibilityTestCase),
					typeof(MDArrayTestCase),
					typeof(NonSerializedAttributeTestCase),
					typeof(ObjectInfoMigration52TestCase),
                    typeof(ObjectInfoMigration57TestCase),
					typeof(ObjectSetAsListTestCase),
				};
		}
	}
}
