/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections.Generic;

using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Linq
{
	internal class QueryBuilderRecorder
	{
		private List<Action<QueryBuilderContext>> _actions = new List<Action<QueryBuilderContext>>();

		public QueryBuilderRecorder()
		{
		}

		public void Add(Action<QueryBuilderContext> action)
		{
			_actions.Add(action);
		}

		public void Execute(IQuery root)
		{
			var context = new QueryBuilderContext(root);
			foreach (var action in _actions)
				action(context);
		}
	}
}
