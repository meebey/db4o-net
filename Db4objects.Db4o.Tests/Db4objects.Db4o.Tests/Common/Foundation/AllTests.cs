namespace Db4objects.Db4o.Tests.Common.Foundation
{
	public class AllTests : Db4oUnit.ITestSuiteBuilder
	{
		public virtual Db4oUnit.TestSuite Build()
		{
			return new Db4oUnit.ReflectionTestSuiteBuilder(new System.Type[] { typeof(Db4objects.Db4o.Tests.Common.Foundation.Algorithms4TestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Foundation.ArrayIterator4TestCase), typeof(Db4objects.Db4o.Tests.Common.Foundation.Arrays4TestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Foundation.BitMap4TestCase), typeof(Db4objects.Db4o.Tests.Common.Foundation.Collection4TestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Foundation.CompositeIterator4TestCase), typeof(Db4objects.Db4o.Tests.Common.Foundation.Hashtable4TestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Foundation.IntArrayListTestCase), typeof(Db4objects.Db4o.Tests.Common.Foundation.Iterable4AdaptorTestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Foundation.IteratorsTestCase), typeof(Db4objects.Db4o.Tests.Common.Foundation.Queue4TestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Foundation.SortedCollection4TestCase), typeof(Db4objects.Db4o.Tests.Common.Foundation.Stack4TestCase)
				, typeof(Db4objects.Db4o.Tests.Common.Foundation.TreeKeyIteratorTestCase), typeof(Db4objects.Db4o.Tests.Common.Foundation.YapReaderTestCase)
				 }).Build();
		}

		public static void Main(string[] args)
		{
			new Db4oUnit.TestRunner(typeof(Db4objects.Db4o.Tests.Common.Foundation.AllTests))
				.Run();
		}
	}
}
