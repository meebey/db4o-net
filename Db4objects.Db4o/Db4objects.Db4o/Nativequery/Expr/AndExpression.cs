using Db4objects.Db4o.Nativequery.Expr;

namespace Db4objects.Db4o.Nativequery.Expr
{
	public class AndExpression : BinaryExpression
	{
		public AndExpression(IExpression left, IExpression right) : base(left, right)
		{
		}

		public override string ToString()
		{
			return "(" + _left + ")&&(" + _right + ")";
		}

		public override void Accept(IExpressionVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
