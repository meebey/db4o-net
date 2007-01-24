namespace Db4objects.Db4o.Tests.Common.Types.Arrays
{
	public class AllTests : Db4oUnit.Extensions.Db4oTestSuite
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Types.Arrays.AllTests().RunSolo();
		}

		protected override System.Type[] TestCases()
		{
			return new System.Type[] { typeof(Db4objects.Db4o.Tests.Common.Types.Arrays.ArrayNOrderTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Types.Arrays.ByteArrayTestCase), typeof(Db4objects.Db4o.Tests.Common.Types.Arrays.NestedArraysTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Types.Arrays.SimpleStringArrayTestCase), typeof(Db4objects.Db4o.Tests.Common.Types.Arrays.SimpleTypeArrayInUntypedVariableTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Types.Arrays.TypedArrayInObjectTestCase), 
				typeof(Db4objects.Db4o.Tests.Common.Types.Arrays.TypedDerivedArrayTestCase) };
		}
	}
}
