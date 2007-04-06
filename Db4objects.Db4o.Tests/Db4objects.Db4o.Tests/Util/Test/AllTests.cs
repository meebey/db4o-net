using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Util.Test;

namespace Db4objects.Db4o.Tests.Util.Test
{
	public class AllTests : Db4oTestSuite
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Util.Test.AllTests().RunSolo();
		}

		protected override Type[] TestCases()
		{
			return new Type[] { typeof(PermutingTestConfigTestCase) };
		}
	}
}
