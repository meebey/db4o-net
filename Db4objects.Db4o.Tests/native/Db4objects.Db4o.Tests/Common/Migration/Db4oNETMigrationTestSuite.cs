using System;
using System.Collections;
using System.Text;
using Db4objects.Db4o.Tests.CLI1.Handlers;

namespace Db4objects.Db4o.Tests.Common.Migration
{
    class Db4oNETMigrationTestSuite : Db4oMigrationTestSuite
    {
        protected override Type[] TestCases()
        {
            ArrayList list = new ArrayList();
            list.AddRange(base.TestCases());

            Type[] netTypes = new Type[] {
                typeof(DateTimeHandlerUpdateTestCase),
                typeof(DecimalHandlerUpdateTestCase),
                typeof(GUIDHandlerUpdateTestCase),
                typeof(SByteHandlerUpdateTestCase),
                typeof(StructHandlerUpdateTestCase),
                typeof(UIntHandlerUpdateTestCase),
                typeof(ULongHandlerUpdateTestCase),
                typeof(UShortHandlerUpdateTestCase),
            };

            list.AddRange(netTypes);
            Type[] allTypes = (Type[]) list.ToArray(typeof(Type));
            return allTypes;
        }
    }
}
