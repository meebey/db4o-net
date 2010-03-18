/* Copyright (C) 2010   Versant Inc.   http://www.db4o.com */

using Db4oTool.Core;

namespace Db4oTool.Tests.TA
{
	partial class TACollectionsTestCase : TATestCaseBase
	{
		public void TestMethodWithInterfaceParameter()
		{
			AssertConstructorInstrumentation("InitInterface");
			AssertConstructorInstrumentation("CollectionInitInterface");
		}

		public void TestLocalsAsInterface()
		{
			AssertConstructorInstrumentation("LocalsAsIList");
			AssertConstructorInstrumentation("CollectionLocalsAsIList");
		}

		public void TestMethodReturningNewListAsInterface()
		{
			AssertConstructorInstrumentation("CreateList");
			AssertConstructorInstrumentation("CollectionCreateList");
		}

		public void TestAssignmentOfConstructorLessListToInterface()
		{
			AssertConstructorInstrumentation("ParameterLessConstructor");
			AssertConstructorInstrumentation("CollectionParameterLessConstructor");
		}

		public void TestConstructorsWarnings()
		{
			AssertConstructorInstrumentationWarning("InitConcrete");
			AssertConstructorInstrumentationWarning("AssignmentOfConcreteListToLocal");
			AssertConstructorInstrumentationWarning("AssignmentOfConcreteListToField");
			AssertConstructorInstrumentationWarning("PublicCreateConcreteList");
		}

		public void TestSuccessfulCasts()
		{
			AssertSuccessfulCast("CastFollowedByParameterLessMethod");
			AssertSuccessfulCast("CastFollowedByMethodWithSingleArgument");
			AssertSuccessfulCast("CastConsumedByPropertyAccess");
		}

		public void TestFailingCasts()
		{
			AssertFailingCast("CastConsumedByLocal");
			AssertFailingCast("CastConsumedByField");
			AssertFailingCast("CastConsumedByArgument");
			AssertFailingCast("CastConsumedByMethodReturn");
		}

		protected override Configuration Configuration(string assemblyLocation)
        {
            Configuration conf = base.Configuration(assemblyLocation);
            conf.PreserveDebugInfo = true;
            return conf;
        }
	}
}
