/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Linq.Expressions;
using System.Reflection;

using Db4objects.Db4o.Linq.Caching;
using Db4objects.Db4o.Linq.Internals;
using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Linq.Expressions
{
	internal class OrderByClauseVisitor : ExpressionQueryBuilder
	{
		private OrderByDirection _direction;

		private static ICachingStrategy<Expression, QueryBuilderRecorder> _cache =
			new SingleItemCachingStrategy<Expression, QueryBuilderRecorder>(ExpressionEqualityComparer.Instance);

		public OrderByClauseVisitor(IQuery query, OrderByDirection direction) : base(query)
		{
			_direction = direction;
		}

		protected override ICachingStrategy<Expression, QueryBuilderRecorder> GetCachingStrategy()
		{
			return _cache;
		}

		protected override void VisitMemberAccess(MemberExpression m)
		{
			if (!IsParameterReference(m)) CannotOptimize(m);

			ProcessMemberAccess(m);

			Recorder.Add(ctx => ApplyDirection(_direction, ctx.CurrentQuery));
		}

		private static void ApplyDirection(OrderByDirection direction, IQuery query)
		{
			switch (direction)
			{
				case OrderByDirection.Ascending:
					query.OrderAscending();
					break;
				case OrderByDirection.Descending:
					query.OrderDescending();
					break;
				default:
					throw new NotSupportedException("Unsupported direction: " + direction);
			}
		}
	}
}
