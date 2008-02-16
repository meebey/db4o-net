/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Db4objects.Db4o.Linq.Internals
{
	internal class PlaceHolderQuery<T> : IDb4oLinqQuery<T>
	{
		private IObjectContainer _container;

		public IObjectContainer Container
		{
			get { return _container; }
		}

		public PlaceHolderQuery(IObjectContainer container)
		{
			_container = container;
		}

		public IEnumerator<T> GetEnumerator()
		{
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
