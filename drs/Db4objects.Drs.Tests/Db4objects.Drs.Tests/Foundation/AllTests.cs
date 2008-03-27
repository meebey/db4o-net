/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests.Foundation
{
	public class AllTests : Db4oUnit.ReflectionTestSuite
	{
		public static void Main(string[] args)
		{
			new Db4objects.Drs.Tests.Foundation.AllTests().Run();
		}

		protected override System.Type[] TestCases()
		{
			return new System.Type[] { typeof(Db4objects.Drs.Tests.Foundation.ObjectSetCollection4FacadeTestCase
				), typeof(Db4objects.Drs.Tests.Foundation.Set4Testcase) };
		}
	}
}
