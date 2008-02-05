/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

using System;

using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Linq.Tests
{
	public class AllTests : Db4oTestSuite
	{
		public static int Main(string[] args)
		{
			return new AllTests().RunSolo();
		}

		protected override Type[] TestCases()
		{
			return new [] {
				typeof(Caching.AllTests),
				typeof(CodeAnalysis.AllTests),
				typeof(Expressions.AllTests),
				typeof(Queries.AllTests),
				typeof(CollectionContainsTestCase),
				typeof(ComposedQueryTestCase),
				typeof(CountTestCase),
				typeof(OrderByTestCase),
				typeof(ParameterizedWhereTestCase),
				typeof(PartiallyOptimizedQueryTestCase),
				typeof(StringMethodTestCase),
				typeof(WhereTestCase),
			};
		}
	}
}
