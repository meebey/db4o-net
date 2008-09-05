using System;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI2.Handlers
{
    public class AllTests : Db4oTestSuite
    {
        protected override Type[] TestCases()
        {
            return new Type[]
                   	{
                   		typeof(TypeHandlerConfigurationTestCase),
						typeof(GenericCollectionTypeHandlerTestSuite),
						typeof(GenericCollectionTypeHandlerGreaterSmallerTestSuite)
                   	};
        }
    }
}
