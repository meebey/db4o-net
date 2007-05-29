/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests
{
	public class AllTests : Db4oTestSuite
	{
		public static int Main(string[] args)
		{
//			return new AllTests().RunSolo();
//            return new AllTests().RunClientServer();
		    return new AllTests().RunAll();
		}
		
		protected override Type[] TestCases()
		{
			return new Type[]
				{
					typeof(Db4objects.Db4o.Tests.Common.Acid.CrashSimulatingTestCase),
					typeof(Db4objects.Db4o.Tests.Common.AllTests),
					typeof(Db4objects.Db4o.Tests.CLI1.AllTests),
					typeof(Db4objects.Db4o.Tests.CLI2.AllTests),
                    typeof(Db4objects.Db4o.Tests.SharpenLang.AllTests),
				};
		}
	}
}
