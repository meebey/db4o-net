using System;
using System.Text;
using Db4objects.Db4o.Tests.Common.Migration;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.Common.Migration
{
    class AllTests : Db4oTestSuite
    {
        protected override Type[] TestCases()
        {
            return new Type[] {
#if !CF_1_0 && !CF_2_0
                typeof(Db4oNETMigrationTestSuite),
#endif
            };
        }
    }
}
