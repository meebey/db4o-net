/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Linq.Expressions;
using System.Reflection;
using Db4objects.Db4o.Internal.Caching;
using Db4objects.Db4o.Linq.Caching;
using Db4objects.Db4o.Linq.Internals;
using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Linq.Expressions
{
	internal class OrderByAscendingClauseVisitor : OrderByClauseVisitorBase
	{
		private static ICachingStrategy<Expression, IQueryBuilderRecord> _cache =
			Cache4CachingStrategy<Expression, IQueryBuilderRecord>.NewInstance(CacheFactory.New2QXCache(10), ExpressionEqualityComparer.Instance);

		protected override ICachingStrategy<Expression, IQueryBuilderRecord> GetCachingStrategy()
		{
			return _cache;
		}

		protected override void ApplyDirection(IQuery query)
		{
			query.OrderAscending();
		}
	}
}
