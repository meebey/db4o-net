namespace Db4objects.Db4o.Nativequery.Expr
{
	public class ComparisonExpression : Db4objects.Db4o.Nativequery.Expr.IExpression
	{
		private Db4objects.Db4o.Nativequery.Expr.Cmp.FieldValue _left;

		private Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperand _right;

		private Db4objects.Db4o.Nativequery.Expr.Cmp.ComparisonOperator _op;

		public ComparisonExpression(Db4objects.Db4o.Nativequery.Expr.Cmp.FieldValue left, 
			Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperand right, Db4objects.Db4o.Nativequery.Expr.Cmp.ComparisonOperator
			 op)
		{
			if (left == null || right == null || op == null)
			{
				throw new System.ArgumentNullException();
			}
			this._left = left;
			this._right = right;
			this._op = op;
		}

		public virtual Db4objects.Db4o.Nativequery.Expr.Cmp.FieldValue Left()
		{
			return _left;
		}

		public virtual Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperand Right()
		{
			return _right;
		}

		public virtual Db4objects.Db4o.Nativequery.Expr.Cmp.ComparisonOperator Op()
		{
			return _op;
		}

		public override string ToString()
		{
			return _left + " " + _op + " " + _right;
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
			Db4objects.Db4o.Nativequery.Expr.ComparisonExpression casted = (Db4objects.Db4o.Nativequery.Expr.ComparisonExpression
				)other;
			return _left.Equals(casted._left) && _right.Equals(casted._right) && _op.Equals(casted
				._op);
		}

		public override int GetHashCode()
		{
			return (_left.GetHashCode() * 29 + _right.GetHashCode()) * 29 + _op.GetHashCode();
		}

		public virtual void Accept(Db4objects.Db4o.Nativequery.Expr.IExpressionVisitor visitor
			)
		{
			visitor.Visit(this);
		}
	}
}
