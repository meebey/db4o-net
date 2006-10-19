namespace Db4objects.Db4o.Nativequery.Expr.Cmp
{
	public interface IComparisonOperandVisitor
	{
		void Visit(Db4objects.Db4o.Nativequery.Expr.Cmp.ArithmeticExpression operand);

		void Visit(Db4objects.Db4o.Nativequery.Expr.Cmp.ConstValue operand);

		void Visit(Db4objects.Db4o.Nativequery.Expr.Cmp.FieldValue operand);

		void Visit(Db4objects.Db4o.Nativequery.Expr.Cmp.Field.CandidateFieldRoot root);

		void Visit(Db4objects.Db4o.Nativequery.Expr.Cmp.Field.PredicateFieldRoot root);

		void Visit(Db4objects.Db4o.Nativequery.Expr.Cmp.Field.StaticFieldRoot root);

		void Visit(Db4objects.Db4o.Nativequery.Expr.Cmp.ArrayAccessValue operand);

		void Visit(Db4objects.Db4o.Nativequery.Expr.Cmp.MethodCallValue value);
	}
}
