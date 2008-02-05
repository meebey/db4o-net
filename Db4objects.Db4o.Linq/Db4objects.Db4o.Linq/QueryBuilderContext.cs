/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections.Generic;

using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Linq
{
	internal class QueryBuilderContext
	{
		private IQuery _root;
		private IQuery _query;
		private Stack<IConstraint> _constraints = new Stack<IConstraint>();

		public IQuery RootQuery
		{
			get { return _root; }
		}

		public IQuery CurrentQuery
		{
			get { return _query; }
		}

		public QueryBuilderContext(IQuery root)
		{
			_root = root;
			_query = _root;
		}

		public void PushQuery(IQuery query)
		{
			_query = query;
		}

		public void PushConstraint(IConstraint constraint)
		{
			_constraints.Push(constraint);
		}

		public IConstraint PopConstraint()
		{
			return _constraints.Pop();
		}

		public void ApplyConstraint(Func<IConstraint, IConstraint> constraint)
		{
			PushConstraint(constraint(PopConstraint()));
		}
	}
}
