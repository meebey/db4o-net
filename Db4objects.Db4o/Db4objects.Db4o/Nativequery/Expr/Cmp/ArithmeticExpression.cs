namespace Db4objects.Db4o.Nativequery.Expr.Cmp
{
	public class ArithmeticExpression : Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperand
	{
		private Db4objects.Db4o.Nativequery.Expr.Cmp.ArithmeticOperator _op;

		private Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperand _left;

		private Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperand _right;

		public ArithmeticExpression(Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperand
			 left, Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperand right, Db4objects.Db4o.Nativequery.Expr.Cmp.ArithmeticOperator
			 op)
		{
			this._op = op;
			this._left = left;
			this._right = right;
		}

		public virtual Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperand Left()
		{
			return _left;
		}

		public virtual Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperand Right()
		{
			return _right;
		}

		public virtual Db4objects.Db4o.Nativequery.Expr.Cmp.ArithmeticOperator Op()
		{
			return _op;
		}

		public override string ToString()
		{
			return "(" + _left + _op + _right + ")";
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null || obj.GetType() != GetType())
			{
				return false;
			}
			Db4objects.Db4o.Nativequery.Expr.Cmp.ArithmeticExpression casted = (Db4objects.Db4o.Nativequery.Expr.Cmp.ArithmeticExpression
				)obj;
			return _left.Equals(casted._left) && _right.Equals(casted._right) && _op.Equals(casted
				._op);
		}

		public override int GetHashCode()
		{
			int hc = _left.GetHashCode();
			hc *= 29 + _right.GetHashCode();
			hc *= 29 + _op.GetHashCode();
			return hc;
		}

		public virtual void Accept(Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperandVisitor
			 visitor)
		{
			visitor.Visit(this);
		}
	}
}
