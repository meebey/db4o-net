/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Db4objects.Db4o.Linq.Expressions;
using Db4objects.Db4o.Linq.Internals;

namespace Db4objects.Db4o.Linq
{
	/// <summary>
	/// A class that defines some standard linq query operations
	/// that can be optimized.
	/// </summary>
	public static class Db4oLinqQueryExtensions
	{
		public static IDb4oLinqQuery<TSource> Where<TSource>(this IDb4oLinqQuery<TSource> self, Expression<Func<TSource, bool>> expression)
		{
			return Process(self,
				query => {
					new WhereClauseVisitor(query.GetUnderlyingQuery()).Process(expression);
				},
				data => {
					return data.Where(expression.Compile());
				});
		}

		public static int Count<TSource>(this IDb4oLinqQuery<TSource> self)
		{
			if (self == null)
				throw new ArgumentNullException("self");

			var query = self as Db4oQuery<TSource>;
			if (query != null)
				return query.GetResult().Count;

			return Enumerable.Count(self);
		}

		private static IDb4oLinqQuery<TSource> Process<TSource>(
			IDb4oLinqQuery<TSource> query,
			Action<Db4oQuery<TSource>> queryProcessor,
			Func<IEnumerable<TSource>, IEnumerable<TSource>> fallbackProcessor)
		{
			if (query == null)
				throw new ArgumentNullException("query");

			var candidate = query as Db4oQuery<TSource>;

			if (candidate == null)
				return new UnoptimizedQuery<TSource>(fallbackProcessor(query));

			try
			{
				queryProcessor(candidate);
				return candidate;
			}
			catch (QueryOptimizationException)
			{
				return new UnoptimizedQuery<TSource>(fallbackProcessor(candidate.GetExtentResult()));
			}
		}

		private static IDb4oLinqQuery<TSource> ProcessOrderBy<TSource, TKey>(IDb4oLinqQuery<TSource> query, OrderByDirection direction, Expression<Func<TSource, TKey>> expression, Func<IEnumerable<TSource>, IEnumerable<TSource>> fallbackProcessor)
		{
			return Process(query,
				q => {
					new OrderByClauseVisitor(q.GetUnderlyingQuery(), direction).Process(expression);
				}, fallbackProcessor);
		}

		public static IDb4oLinqQuery<TSource> OrderBy<TSource, TKey>(this IDb4oLinqQuery<TSource> self, Expression<Func<TSource, TKey>> expression)
		{
			return ProcessOrderBy(self, OrderByDirection.Ascending, expression,
				data => {
					return data.OrderBy(expression.Compile());
				});
		}

		public static IDb4oLinqQuery<TSource> OrderByDescending<TSource, TKey>(this IDb4oLinqQuery<TSource> self, Expression<Func<TSource, TKey>> expression)
		{
			return ProcessOrderBy(self, OrderByDirection.Descending, expression,
				data => {
					return data.OrderByDescending(expression.Compile());
				});
		}

		public static IDb4oLinqQuery<TSource> ThenBy<TSource, TKey>(this IDb4oLinqQuery<TSource> self, Expression<Func<TSource, TKey>> expression)
		{
			return ProcessOrderBy(self, OrderByDirection.Ascending, expression,
				data => {
					var ordered = data as IOrderedEnumerable<TSource>;
					if (ordered != null) return ordered.ThenBy(expression.Compile());

					throw new NotSupportedException("Cannot fallback in a thenby");
				});
		}

		public static IDb4oLinqQuery<TSource> ThenByDescending<TSource, TKey>(this IDb4oLinqQuery<TSource> self, Expression<Func<TSource, TKey>> expression)
		{
			return ProcessOrderBy(self, OrderByDirection.Descending, expression,
				data => {
					var ordered = data as IOrderedEnumerable<TSource>;
					if (ordered != null) return ordered.ThenByDescending(expression.Compile());

					throw new NotSupportedException("Cannot fallback in a thenby");
				});
		}

		public static IDb4oLinqQuery<TRet> Select<TSource, TRet>(this IDb4oLinqQuery<TSource> self, Func<TSource, TRet> selector)
		{
			var temp = self as PlaceHolderQuery<TSource>;
			if (temp == null) return new UnoptimizedQuery<TRet>(Enumerable.Select(self, selector));
			return new Db4oQuery<TRet>(temp.Container);
		}
	}
}
