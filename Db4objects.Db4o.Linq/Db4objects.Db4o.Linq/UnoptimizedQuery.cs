/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Db4objects.Db4o.Linq
{
	internal class UnoptimizedQuery<T> : IDb4oLinqQuery<T>
	{
		IEnumerable<T> _result;

		public UnoptimizedQuery(IEnumerable<T> result)
		{
			if (result == null)
				throw new ArgumentNullException("result");

			_result = result;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _result.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
