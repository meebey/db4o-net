/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;
using Db4objects.Db4o.Tests.Common.Reflect.Custom;
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
//			return new Common.Reflect.Custom.AllTests().RunSolo();
//			return new AllTests().RunSolo();
//			return new AllTests().RunClientServer();
//			return new AllTestsConcurrency().RunConcurrencyAll();
		    return new AllTests().RunAll();
#endif
		}
		
		protected override Type[] TestCases()
		{
//			return new Type[] { typeof(CLI2.Collections.GenericListTypeHandlerTestCase) };
			//return new Type[] { typeof(Compact.UnoptimizedLinqTestCase), };
			return new Type[]
				{	
#if CF_3_5
					typeof(Compact.AllTests),
#endif
//					typeof(Db4objects.Db4o.Tests.CLI2.TA.NullableTypeActivationTestCase),
					typeof(Db4objects.Db4o.Tests.Common.Migration.AllTests),
                    typeof(Db4objects.Db4o.Tests.Common.TA.AllTests),
                    typeof(Db4objects.Db4o.Tests.Common.AllTests),
                    typeof(Db4objects.Db4o.Tests.CLI1.AllTests),
					typeof(Db4objects.Db4o.Tests.CLI2.AllTests),
                    typeof(Db4objects.Db4o.Tests.SharpenLang.AllTests),
					typeof(AllTestsConcurrency),
				};
		}
	}
}
