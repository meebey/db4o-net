/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4oUnit;
using Db4objects.Db4o.Tests.Common.Foundation;

namespace Db4objects.Db4o.Tests.Common.Foundation
{
	public class AllTests : ITestSuiteBuilder
	{
		public virtual IEnumerator GetEnumerator()
		{
			return new ReflectionTestSuiteBuilder(new Type[] { typeof(Algorithms4TestCase), typeof(
				ArrayIterator4TestCase), typeof(Arrays4TestCase), typeof(BitMap4TestCase), typeof(
				BlockingQueueTestCase), typeof(BufferTestCase), typeof(CircularBufferTestCase), 
				typeof(Collection4TestCase), typeof(CompositeIterator4TestCase), typeof(CoolTestCase
				), typeof(DynamicVariableTestCase), typeof(Hashtable4TestCase), typeof(IntArrayListTestCase
				), typeof(IntMatcherTestCase), typeof(Iterable4AdaptorTestCase), typeof(IteratorsTestCase
				), typeof(Map4TestCase), typeof(NoDuplicatesQueueTestCase), typeof(NonblockingQueueTestCase
				), typeof(ObjectPoolTestCase), typeof(Path4TestCase), typeof(SortedCollection4TestCase
				), typeof(Stack4TestCase), typeof(TreeKeyIteratorTestCase), typeof(TreeNodeIteratorTestCase
				) }).GetEnumerator();
		}

		public static void Main(string[] args)
		{
			new ConsoleTestRunner(typeof(Db4objects.Db4o.Tests.Common.Foundation.AllTests)).Run
				();
		}
	}
}
