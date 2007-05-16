/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Nativequery.Expr.Cmp;
using Db4objects.Db4o.Nativequery.Expr.Cmp.Field;

namespace Db4objects.Db4o.Nativequery.Expr.Cmp
{
	public interface IComparisonOperandVisitor
	{
		void Visit(ArithmeticExpression operand);

		void Visit(ConstValue operand);

		void Visit(FieldValue operand);

		void Visit(CandidateFieldRoot root);

		void Visit(PredicateFieldRoot root);

		void Visit(StaticFieldRoot root);

		void Visit(ArrayAccessValue operand);

		void Visit(MethodCallValue value);
	}
}
