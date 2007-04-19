using Db4objects.Db4o.Nativequery.Expr;

namespace Db4objects.Db4o.Nativequery.Expr
{
	public class NotExpression : IExpression
	{
		private IExpression _expr;

		public NotExpression(IExpression expr)
		{
			this._expr = expr;
		}

		public override string ToString()
		{
			return "!(" + _expr + ")";
		}

		public virtual IExpression Expr()
		{
			return _expr;
		}

		public override bool Equals(object other)
		{
			if (this == other)
			{
				return true;
			}
			if (other == null || GetType() != other.GetType())
			{
				return false;
			}
			Db4objects.Db4o.Nativequery.Expr.NotExpression casted = (Db4objects.Db4o.Nativequery.Expr.NotExpression
				)other;
			return _expr.Equals(casted._expr);
		}

		public override int GetHashCode()
		{
			return -_expr.GetHashCode();
		}

		public virtual void Accept(IExpressionVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
