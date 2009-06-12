using System;
using System.Collections;
using System.Text;
using Db4objects.Db4o.Tests.CLI1.Handlers;
using System.IO;
using Db4objects.Db4o.Tests.Common.Handlers;
using Db4objects.Db4o.Tests.Util;
using Db4oUnit;
using Db4objects.Db4o.Tests.CLI2.Handlers;

namespace Db4objects.Db4o.Tests.Common.Migration
{
#if !CF && !SILVERLIGHT
    class Db4oNETMigrationTestSuite : Db4oMigrationTestSuite
    {
//		override protected string[] Libraries()
//		{
//			return new string[] { AssemblyPathFor("7.8") };
//		}

    	private string AssemblyPathFor(string version)
    	{
    		return WorkspaceServices.WorkspacePath("db4o.archives/net-2.0/" + version + "/Db4objects.Db4o.dll");
    	}

    	protected override Type[] TestCases()
        {
			// return new Type[] { typeof(EnumHandlerUpdateTestCase), };

            if (!Directory.Exists(Db4oLibrarian.LibraryPath()))
            {
                TestPlatform.GetStdErr().WriteLine("DISABLED: " + GetType());
                return new Type[] { };
            }

            ArrayList list = new ArrayList();
            list.AddRange(base.TestCases());

            Type[] netTypes = new Type[] {
                typeof(SimplestPossibleHandlerUpdateTestCase),
                typeof(GenericListVersionUpdateTestCase),
                typeof(GenericDictionaryVersionUpdateTestCase),
                typeof(DateTimeHandlerUpdateTestCase),
                typeof(IndexedDateTimeUpdateTestCase),
                typeof(DecimalHandlerUpdateTestCase),
				typeof(EnumHandlerUpdateTestCase),
                typeof(GUIDHandlerUpdateTestCase),
                typeof(HashtableUpdateTestCase),
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
