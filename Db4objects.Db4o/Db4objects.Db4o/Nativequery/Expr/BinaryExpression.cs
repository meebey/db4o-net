namespace Db4objects.Db4o.Nativequery.Expr
{
	public abstract class BinaryExpression : Db4objects.Db4o.Nativequery.Expr.IExpression
	{
		protected Db4objects.Db4o.Nativequery.Expr.IExpression _left;

		protected Db4objects.Db4o.Nativequery.Expr.IExpression _right;

		public BinaryExpression(Db4objects.Db4o.Nativequery.Expr.IExpression left, Db4objects.Db4o.Nativequery.Expr.IExpression
			 right)
		{
			this._left = left;
			this._right = right;
		}

		public virtual Db4objects.Db4o.Nativequery.Expr.IExpression Left()
		{
			return _left;
		}

		public virtual Db4objects.Db4o.Nativequery.Expr.IExpression Right()
		{
			return _right;
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
			Db4objects.Db4o.Nativequery.Expr.BinaryExpression casted = (Db4objects.Db4o.Nativequery.Expr.BinaryExpression
				)other;
			return _left.Equals(casted._left) && (_right.Equals(casted._right)) || _left.Equals
				(casted._right) && (_right.Equals(casted._left));
		}

		public override int GetHashCode()
		{
			return _left.GetHashCode() + _right.GetHashCode();
		}

		public abstract void Accept(Db4objects.Db4o.Nativequery.Expr.IExpressionVisitor arg1
			);
	}
}
