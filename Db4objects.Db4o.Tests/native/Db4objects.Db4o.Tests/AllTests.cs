/* Copyright (C) 2007   Versant Inc.   http://www.db4o.com */
using System;
using Db4objects.Db4o.Tests.CLI2.Assorted;
using Db4objects.Db4o.Tests.Common.Migration;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;

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
//			return RunInMemory();
//			return new AllTests().RunSolo();
//			return new AllTests().RunClientServer();
//			return new AllTestsConcurrency().RunConcurrencyAll();
//			return new Jre12.Collections.BigSetTestCase().RunAll();
		    return new AllTests().RunAll();
#endif
		}

		private static int RunInMemory()
		{
			return new ConsoleTestRunner(new Db4oTestSuiteBuilder(new Db4oInMemory(), typeof(AllTests))).Run();
		}
		
		protected override Type[] TestCases()
		{
//			return new Type[] { typeof(Db4objects.Db4o.Tests.Common.AllTests) };
			return new Type[]
				{	
#if CF_3_5
					typeof(Db4objects.Db4o.Linq.Tests.AllTests),
#endif
                    typeof(Db4objects.Db4o.Tests.Common.Migration.AllTests),
                    typeof(Db4objects.Db4o.Tests.Common.TA.AllTests),
                    typeof(Db4objects.Db4o.Tests.Common.AllTests),
                    typeof(Db4objects.Db4o.Tests.Jre5.Annotation.AllTests),
                    typeof(Db4objects.Db4o.Tests.Jre5.Collections.Typehandler.AllTests),
                    typeof(Db4objects.Db4o.Tests.CLI1.AllTests),
                    typeof(Db4objects.Db4o.Tests.CLI2.AllTests),
                    typeof(Db4objects.Db4o.Tests.SharpenLang.AllTests),
#if SILVERLIGHT
                    typeof(Db4objects.Db4o.Tests.Silverlight.AllTests),
#endif
                    typeof(AllTestsConcurrency),
				};
		}
	}
}
