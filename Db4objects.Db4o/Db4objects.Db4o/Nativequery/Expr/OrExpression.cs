namespace Db4objects.Db4o.Nativequery.Expr
{
	public class OrExpression : Db4objects.Db4o.Nativequery.Expr.BinaryExpression
	{
		public OrExpression(Db4objects.Db4o.Nativequery.Expr.IExpression left, Db4objects.Db4o.Nativequery.Expr.IExpression
			 right) : base(left, right)
		{
		}

		public override string ToString()
		{
			return "(" + _left + ")||(" + _right + ")";
		}

		public override void Accept(Db4objects.Db4o.Nativequery.Expr.IExpressionVisitor visitor
			)
		{
			visitor.Visit(this);
		}
	}
}
