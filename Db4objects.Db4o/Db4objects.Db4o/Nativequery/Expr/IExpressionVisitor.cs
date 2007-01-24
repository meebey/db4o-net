namespace Db4objects.Db4o.Nativequery.Expr
{
	public interface IExpressionVisitor
	{
		void Visit(Db4objects.Db4o.Nativequery.Expr.AndExpression expression);

		void Visit(Db4objects.Db4o.Nativequery.Expr.OrExpression expression);

		void Visit(Db4objects.Db4o.Nativequery.Expr.NotExpression expression);

		void Visit(Db4objects.Db4o.Nativequery.Expr.ComparisonExpression expression);

		void Visit(Db4objects.Db4o.Nativequery.Expr.BoolConstExpression expression);
	}
}
