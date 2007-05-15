/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;

namespace Db4objects.Db4o.Tests.CLI1.Aliases
{
    public class AllTests : Db4oUnit.Extensions.Db4oTestSuite
    {
        protected override Type[] TestCases()
        {
            return new System.Type[]
				{
                    typeof(JavaFromNetAliasesTestCase),
                    typeof(JavaFromNetWithSameNameTestCase),
                    typeof(TypeAliasesTestCase),
				};
        }
    }
}
