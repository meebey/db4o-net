/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Linq.Expressions;
using System.Reflection;

using Db4objects.Db4o.Linq.Caching;
using Db4objects.Db4o.Linq.Internals;
using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Linq.Expressions
{
	internal abstract class OrderByClauseVisitorBase : ExpressionQueryBuilder
	{
		protected abstract void ApplyDirection(IQuery query);

		protected override void VisitMemberAccess(MemberExpression m)
		{
			if (!IsParameterReference(m)) CannotOptimize(m);

			ProcessMemberAccess(m);

			Recorder.Add(ctx => ApplyDirection(ctx.CurrentQuery));
		}
	}
}
