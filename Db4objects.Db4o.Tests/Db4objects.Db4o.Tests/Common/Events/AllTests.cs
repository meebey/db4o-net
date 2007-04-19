using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Events;

namespace Db4objects.Db4o.Tests.Common.Events
{
	public class AllTests : Db4oTestSuite
	{
		protected override Type[] TestCases()
		{
			return new Type[] { typeof(ActivationEventsTestCase), typeof(ClassRegistrationEventsTestCase)
				, typeof(InstantiationEventsTestCase) };
		}
	}
}
