namespace Db4objects.Db4o.Tests.Common.Soda
{
	public class AllTests : Db4oUnit.Extensions.Db4oTestSuite
	{
		protected override System.Type[] TestCases()
		{
			return new System.Type[] { typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.AllTests)
				, typeof(Db4objects.Db4o.Tests.Common.Soda.CollectionIndexedJoinTestCase), typeof(Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STBooleanTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STBooleanWUTestCase), 
				typeof(Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STByteTestCase), typeof(Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STByteWUTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STCharTestCase), typeof(Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STCharWUTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STDoubleTestCase), typeof(Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STDoubleWUTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy.STETH1TestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STFloatTestCase), typeof(Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STFloatWUTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STIntegerTestCase), typeof(Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STIntegerWUTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STLongTestCase), typeof(Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STLongWUTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Soda.Joins.Typed.STOrTTestCase), typeof(Db4objects.Db4o.Tests.Common.Soda.Joins.Untyped.STOrUTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Soda.Ordered.STOStringTestCase), typeof(Db4objects.Db4o.Tests.Common.Soda.Ordered.STOIntegerTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Soda.Ordered.STOIntegerWTTestCase), typeof(Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy.STRTH1TestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy.STSDFT1TestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STShortTestCase), typeof(Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STShortWUTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase), typeof(Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STRUH1TestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy.STTH1TestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH1TestCase)
				 };
		}

		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Soda.AllTests().RunSolo();
		}
	}
}
