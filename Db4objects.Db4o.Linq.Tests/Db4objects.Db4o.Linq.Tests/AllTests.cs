﻿/* Copyright (C) 2007 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Linq.Tests
{
	public class AllTests : Db4oTestSuite
	{
		public static int Main(string[] args)
		{
			var res = new AllTests().RunSolo();
			return res;
		}

		protected override Type[] TestCases()
		{
			return new [] {
				typeof(Caching.AllTests),
				typeof(CodeAnalysis.AllTests),
				typeof(Expressions.AllTests),
				typeof(Queries.AllTests),
				typeof(EnumComparisonTestCase),
				typeof(CollectionContainsOptimizationTestCase),
				typeof(CollectionContainsTestCase),
				typeof(ComposedQueryTestCase),
				typeof(CountTestCase),
				typeof(OrderByTestCase),
				typeof(ParameterizedWhereTestCase),
				typeof(PartiallyOptimizedQueryTestCase),
				typeof(QueryableTestCase),
				typeof(QueryReuseTestCase),
				typeof(StringMethodTestCase),
				typeof(VisualBasicTestCase),
				typeof(WhereTestCase),
				typeof(QueryTranslationPerformanceTestCase),
#if !CF
				typeof(DateTimeOffsetQueryTestCase),
				typeof(GenericQueryTestCase),
#endif
			};
		}
	}
}
