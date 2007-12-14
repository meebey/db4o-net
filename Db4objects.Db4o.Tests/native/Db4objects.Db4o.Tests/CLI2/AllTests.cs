/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
namespace Db4objects.Db4o.Tests.CLI2
{
	public class AllTests : Db4oUnit.Extensions.Db4oTestSuite
	{
		protected override System.Type[] TestCases()
		{
			return new System.Type[]
			    {
			    	typeof(Db4objects.Db4o.Tests.NativeQueries.Diagnostics.NativeQueryOptimizerDiagnosticsTestCase),
                    typeof(Db4objects.Db4o.Tests.CLI2.Assorted.AllTests),
			        typeof(Db4objects.Db4o.Tests.CLI2.Collections.AllTests),
                    typeof(Db4objects.Db4o.Tests.CLI2.Regression.AllTests),
                    typeof(Db4objects.Db4o.Tests.CLI2.TA.AllTests),
			    };
		}
	}
}
