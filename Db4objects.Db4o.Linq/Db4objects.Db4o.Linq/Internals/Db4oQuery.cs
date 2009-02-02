/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Db4objects.Db4o;
using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Linq.Internals
{
	internal class Db4oQuery<T> : IDb4oLinqQueryInternal<T>
	{
		private readonly ISodaQueryFactory _queryFactory;
		private readonly IQueryBuilderRecord _record;

		public Db4oQuery(ISodaQueryFactory queryFactory)
		{
			if (queryFactory == null) throw new ArgumentNullException("queryFactory");
			_queryFactory = queryFactory;
			_record = NullQueryBuilderRecord.Instance;
		}

		public Db4oQuery(Db4oQuery<T> parent, IQueryBuilderRecord record)
		{			
			_queryFactory = parent.QueryFactory;
			_record = new CompositeQueryBuilderRecord(parent.Record, record);
		}

		public ISodaQueryFactory QueryFactory
		{
			get { return _queryFactory; }
		}

		public IQueryBuilderRecord Record
		{
			get { return _record; }
		}

		public int Count
		{
			get { return Execute().Count; }
		}

		public ObjectSetWrapper<T> GetExtentResult()
		{
			var query = NewQuery();
			return Wrap(query.Execute());
		}

		private IQuery NewQuery()
		{
			var query = _queryFactory.Query();
			query.Constrain(typeof(T));
			return query;
		}

		static ObjectSetWrapper<T> Wrap(IObjectSet set)
		{
			return new ObjectSetWrapper<T>(set);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return Wrap(Execute()).GetEnumerator();
		}

		private IObjectSet Execute()
		{
			var query = NewQuery();
			_record.Playback(query);
			return query.Execute();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#region IDb4oLinqQueryInternal<T> Members

		public IEnumerable<T> UnoptimizedThenBy<TKey>(Func<T, TKey> function)
		{
			throw new NotImplementedException("cannot fallback on UnoptimizedThenBy");
		}

		public IEnumerable<T> UnoptimizedThenByDescending<TKey>(Func<T, TKey> function)
		{
			throw new NotImplementedException("cannot fallback on UnoptimizedThenBy");
			/*
			IOrderByRecord record = _orderByRecord;
			IOrderedEnumerable<T> ordered = record.OrderBy(this);

			record = record.Next;
			while (record != null)
			{
				ordered = record.ThenBy(record);
			}
			return ordered.ThenByDescending(function);
			 * */
		}

		public IEnumerable<T> UnoptimizedWhere(Func<T, bool> func)
		{
			return GetExtentResult().Where(func);
		}

		#endregion
	}
}
