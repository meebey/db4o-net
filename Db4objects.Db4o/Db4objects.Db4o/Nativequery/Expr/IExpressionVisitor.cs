/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Nativequery.Expr;

namespace Db4objects.Db4o.Nativequery.Expr
{
	public interface IExpressionVisitor
	{
		void Visit(AndExpression expression);

		void Visit(OrExpression expression);

		void Visit(NotExpression expression);

		void Visit(ComparisonExpression expression);

		void Visit(BoolConstExpression expression);
	}
}
