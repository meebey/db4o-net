/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Linq.Expressions;
using System.Reflection;

using Db4objects.Db4o.Linq.Caching;
using Db4objects.Db4o.Linq.Internals;
using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Linq.Expressions
{
	internal class OrderByDescendingClauseVisitor : OrderByClauseVisitorBase
	{
		private static ICachingStrategy<Expression, IQueryBuilderRecord> _cache =
			new SingleItemCachingStrategy<Expression, IQueryBuilderRecord>(ExpressionEqualityComparer.Instance);

		protected override ICachingStrategy<Expression, IQueryBuilderRecord> GetCachingStrategy()
		{
			return _cache;
		}

		protected override void ApplyDirection(IQuery query)
		{
			query.OrderDescending();
		}
	}
}
