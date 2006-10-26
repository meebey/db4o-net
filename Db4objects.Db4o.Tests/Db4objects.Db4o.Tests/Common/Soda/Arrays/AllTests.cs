namespace Db4objects.Db4o.Tests.Common.Soda.Arrays
{
	public class AllTests : Db4oUnit.Extensions.Db4oTestSuite
	{
		protected override System.Type[] TestCases()
		{
			return new System.Type[] { typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.Untyped.STArrMixedTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.Object.STArrStringOTestCase), 
				typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.Object.STArrStringONTestCase), typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.Typed.STArrStringTTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.Typed.STArrStringTNTestCase), 
				typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.Untyped.STArrStringUTestCase), typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.Untyped.STArrStringUNTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.Object.STArrIntegerOTestCase), 
				typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.Object.STArrIntegerONTestCase), 
				typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.Typed.STArrIntegerTTestCase), typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.Typed.STArrIntegerTNTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.Untyped.STArrIntegerUTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.Untyped.STArrIntegerUNTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.Typed.STArrIntegerWTTestCase), 
				typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.Object.STArrIntegerWTONTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.Object.STArrIntegerWUONTestCase)
				 };
		}

		internal static int Main(string[] args)
		{
			return new Db4objects.Db4o.Tests.Common.Soda.Arrays.AllTests().RunSolo();
		}
	}
}
