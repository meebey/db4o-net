using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Reflect;

namespace Db4objects.Db4o.Tests.Common.Reflect
{
	public class AllTests : Db4oTestSuite
	{
		protected override Type[] TestCases()
		{
			return new Type[] { typeof(GenericReflectorStateTest), typeof(ReflectArrayTestCase)
				 };
		}

		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Reflect.AllTests().RunSolo();
		}
	}
}
