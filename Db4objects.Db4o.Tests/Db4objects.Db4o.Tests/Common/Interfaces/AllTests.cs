using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Interfaces;

namespace Db4objects.Db4o.Tests.Common.Interfaces
{
	public class AllTests : Db4oTestSuite
	{
		protected override Type[] TestCases()
		{
			return new Type[] { typeof(InterfaceTestCase) };
		}
	}
}
