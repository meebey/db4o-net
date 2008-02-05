/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using System.Collections.Generic;

using Db4objects.Db4o;
using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Linq
{
	internal class Db4oQuery<T> : IDb4oLinqQuery<T>
	{
		private IObjectContainer _container;
		private IQuery _query;
		private ObjectSetWrapper<T> _result;

		public Db4oQuery(IObjectContainer container)
		{
			if (container == null)
				throw new ArgumentNullException("container");

			_container = container;
			_query = container.Query();
			_query.Constrain(typeof(T));
		}

		public IQuery GetUnderlyingQuery()
		{
			return _query;
		}

		public ObjectSetWrapper<T> GetResult()
		{
			if (_result != null) return _result;

			return _result = Wrap(_query.Execute());
		}

		public ObjectSetWrapper<T> GetExtentResult()
		{
			var query = _container.Query();
			query.Constrain(typeof(T));
			return Wrap(query.Execute());
		}

		static ObjectSetWrapper<T> Wrap(IObjectSet set)
		{
			return new ObjectSetWrapper<T>(set);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return GetResult().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
