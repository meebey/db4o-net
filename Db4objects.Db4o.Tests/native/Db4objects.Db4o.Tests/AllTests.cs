/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;
using Db4objects.Db4o.Tests.Common.Migration;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests
{
	public class AllTests : Db4oTestSuite
	{
		public static int Main(string[] args)
		{
#if CF
			return new AllTests().RunSolo();
//            return new AllTests().RunClientServer();
#else
//			return new Common.Assorted.IndexCreateDropTestCase().RunSolo();
//			return new Common.Migration.AllTests().RunSolo();
//			return new Common.Refle ct.Custom.AllTests().RunSolo();
//			return new AllTests().RunSolo();
//			return new AllTests().RunClientServer();
//			return new AllTestsConcurrency().RunConcurrencyAll();
		    return new AllTests().RunAll();
#endif
		}
		
		protected override Type[] TestCases()
		{
			//return new Type[] { typeof(CLI1.CrossPlatform.CrossplatformTestCase) };

			return new Type[]
				{	
//					typeof(Db4objects.Db4o.Tests.CLI2.TA.NullableTypeActivationTestCase),

                    // typeof(Db4oMigrationTestSuite)
                     // typeof(Db4oNETMigrationTestSuite)

                    typeof(Common.Migration.AllTests),
                    typeof(Common.TA.AllTests),
                    typeof(Common.AllTests),
					typeof(CLI2.Foundation.AllTests),
					typeof(Jre5.Collections.Typehandler.AllTests),
                    typeof(CLI1.AllTests),
                    typeof(CLI2.AllTests),
                    typeof(SharpenLang.AllTests),
                    typeof(AllTestsConcurrency),
				};
		}
	}
}
