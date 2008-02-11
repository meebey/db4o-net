/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o.Tests.Common.Foundation;

namespace Db4objects.Db4o.Tests.Common.Foundation
{
	public class AllTests : ITestSuiteBuilder
	{
		public virtual TestSuite Build()
		{
			return new ReflectionTestSuiteBuilder(new Type[] { typeof(Algorithms4TestCase), typeof(
				ArrayIterator4TestCase), typeof(Arrays4TestCase), typeof(BitMap4TestCase), typeof(
				BlockingQueueTestCase), typeof(Collection4TestCase), typeof(CompositeIterator4TestCase
				), typeof(ContextVariableTestCase), typeof(CoolTestCase), typeof(Hashtable4TestCase
				), typeof(IntArrayListTestCase), typeof(IntMatcherTestCase), typeof(Iterable4AdaptorTestCase
				), typeof(IteratorsTestCase), typeof(NoDuplicatesQueueTestCase), typeof(NonblockingQueueTestCase
				), typeof(Path4TestCase), typeof(SortedCollection4TestCase), typeof(Stack4TestCase
				), typeof(TreeKeyIteratorTestCase), typeof(TreeNodeIteratorTestCase), typeof(BufferTestCase
				) }).Build();
		}

		public static void Main(string[] args)
		{
			new TestRunner(typeof(Db4objects.Db4o.Tests.Common.Foundation.AllTests)).Run();
		}
	}
}
