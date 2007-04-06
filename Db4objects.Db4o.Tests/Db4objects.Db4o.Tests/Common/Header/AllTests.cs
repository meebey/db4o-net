using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Header;

namespace Db4objects.Db4o.Tests.Common.Header
{
	public class AllTests : Db4oTestSuite
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Header.AllTests().RunSolo();
		}

		protected override Type[] TestCases()
		{
			return new Type[] { typeof(OldHeaderTest), typeof(ConfigurationSettingsTestCase), 
				typeof(IdentityTestCase), typeof(SimpleTimeStampIdTestCase) };
		}
	}
}
