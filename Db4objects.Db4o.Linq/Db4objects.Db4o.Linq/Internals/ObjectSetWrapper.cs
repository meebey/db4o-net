/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections.Generic;

using Db4objects.Db4o;

namespace Db4objects.Db4o.Linq.Internals
{
	public class ObjectSetWrapper<T> : ObjectSequence<T>
	{
		private IObjectSet _set;

		public int Count
		{
			get { return _set.Count; }
		}

		public ObjectSetWrapper(IObjectSet set) : base(set)
		{
			_set = set;
		}
	}
}
