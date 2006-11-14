namespace Db4objects.Db4o.Tests.Common.Reflect
{
	public class AllTests : Db4oUnit.Extensions.Db4oTestSuite
	{
		protected override System.Type[] TestCases()
		{
			return new System.Type[] { typeof(Db4objects.Db4o.Tests.Common.Reflect.ReflectArrayTestCase)
				 };
		}

		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Reflect.AllTests().RunSolo();
		}
	}
}
