using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Acid;

namespace Db4objects.Db4o.Tests.Common.Acid
{
	public class AllTests : Db4oTestSuite
	{
		public static int Main(string[] args)
		{
			return new Db4objects.Db4o.Tests.Common.Acid.AllTests().RunSolo();
		}

		protected override Type[] TestCases()
		{
			return new Type[] { typeof(CrashSimulatingTestCase) };
		}
	}
}
