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
#if CF_1_0 || CF_2_0
			return new AllTests().RunSolo();
//            return new AllTests().RunClientServer();
#else
			return new AllTests().RunSolo();
		    return new AllTests().RunAll();
#endif
		}
		
		protected override Type[] TestCases()
		{
			return new Type[]
				{	
//					typeof(Db4objects.Db4o.Tests.Common.Migration.AllTests),
                    typeof(Db4objects.Db4o.Tests.Common.AllTests),
                    typeof(Db4objects.Db4o.Tests.CLI1.AllTests),
					typeof(Db4objects.Db4o.Tests.CLI2.AllTests),
                    typeof(Db4objects.Db4o.Tests.SharpenLang.AllTests),
				};
		}
	}
}
