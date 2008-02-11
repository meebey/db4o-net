using System;
using System.Collections;
using System.Text;
using Db4objects.Db4o.Tests.CLI1.Handlers;
using System.IO;
using Db4oUnit;

namespace Db4objects.Db4o.Tests.Common.Migration
{
#if !CF
    class Db4oNETMigrationTestSuite : Db4oMigrationTestSuite
    {
        protected override Type[] TestCases()
        {
            if (!Directory.Exists(Db4oLibrarian.LibraryPath()))
            {
                TestPlatform.GetStdErr().WriteLine("DISABLED: " + GetType());
                return new Type[] { };
            }
            ArrayList list = new ArrayList();
            list.AddRange(base.TestCases());

            Type[] netTypes = new Type[] {
                typeof(DateTimeHandlerUpdateTestCase),
                typeof(DecimalHandlerUpdateTestCase),
                typeof(GUIDHandlerUpdateTestCase),
                typeof(NestedStructHandlerUpdateTestCase),
                typeof(SByteHandlerUpdateTestCase),
                typeof(StructHandlerUpdateTestCase),
                typeof(UIntHandlerUpdateTestCase),
                typeof(ULongHandlerUpdateTestCase),
                typeof(UShortHandlerUpdateTestCase),
            };

            list.AddRange(netTypes);
        	return (Type[]) list.ToArray(typeof(Type));
        }
    }
#endif
}
