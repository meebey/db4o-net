using System;
using System.Collections.Generic;
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
                typeof(Db4oNETMigrationTestSuite),
            };
        }
    }
}
