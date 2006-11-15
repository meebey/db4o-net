namespace Db4objects.Db4o.Nativequery.Expr.Build
{
	public class ExpressionBuilder
	{
		/// <summary>Optimizations: !(Bool)->(!Bool), !!X->X, !(X==Bool)->(X==!Bool)</summary>
		public virtual Db4objects.Db4o.Nativequery.Expr.IExpression Not(Db4objects.Db4o.Nativequery.Expr.IExpression
			 expr)
		{
			if (expr.Equals(Db4objects.Db4o.Nativequery.Expr.BoolConstExpression.TRUE))
			{
				return Db4objects.Db4o.Nativequery.Expr.BoolConstExpression.FALSE;
			}
			if (expr.Equals(Db4objects.Db4o.Nativequery.Expr.BoolConstExpression.FALSE))
			{
				return Db4objects.Db4o.Nativequery.Expr.BoolConstExpression.TRUE;
			}
			if (expr is Db4objects.Db4o.Nativequery.Expr.NotExpression)
			{
				return ((Db4objects.Db4o.Nativequery.Expr.NotExpression)expr).Expr();
			}
			if (expr is Db4objects.Db4o.Nativequery.Expr.ComparisonExpression)
			{
				Db4objects.Db4o.Nativequery.Expr.ComparisonExpression cmpExpr = (Db4objects.Db4o.Nativequery.Expr.ComparisonExpression
					)expr;
				if (cmpExpr.Right() is Db4objects.Db4o.Nativequery.Expr.Cmp.ConstValue)
				{
					Db4objects.Db4o.Nativequery.Expr.Cmp.ConstValue rightConst = (Db4objects.Db4o.Nativequery.Expr.Cmp.ConstValue
						)cmpExpr.Right();
					if (rightConst.Value() is bool)
					{
						bool boolVal = (bool)rightConst.Value();
						return new Db4objects.Db4o.Nativequery.Expr.ComparisonExpression(cmpExpr.Left(), 
							new Db4objects.Db4o.Nativequery.Expr.Cmp.ConstValue(!boolVal), cmpExpr.Op());
					}
				}
			}
			return new Db4objects.Db4o.Nativequery.Expr.NotExpression(expr);
		}

		/// <summary>Optimizations: f&&X->f, t&&X->X, X&&X->X, X&&!X->f</summary>
		public virtual Db4objects.Db4o.Nativequery.Expr.IExpression And(Db4objects.Db4o.Nativequery.Expr.IExpression
			 left, Db4objects.Db4o.Nativequery.Expr.IExpression right)
		{
			if (left.Equals(Db4objects.Db4o.Nativequery.Expr.BoolConstExpression.FALSE) || right
				.Equals(Db4objects.Db4o.Nativequery.Expr.BoolConstExpression.FALSE))
			{
				return Db4objects.Db4o.Nativequery.Expr.BoolConstExpression.FALSE;
			}
			if (left.Equals(Db4objects.Db4o.Nativequery.Expr.BoolConstExpression.TRUE))
			{
				return right;
			}
			if (right.Equals(Db4objects.Db4o.Nativequery.Expr.BoolConstExpression.TRUE))
			{
				return left;
			}
			if (left.Equals(right))
			{
				return left;
			}
			if (Negatives(left, right))
			{
				return Db4objects.Db4o.Nativequery.Expr.BoolConstExpression.FALSE;
			}
			return new Db4objects.Db4o.Nativequery.Expr.AndExpression(left, right);
		}

		/// <summary>Optimizations: X||t->t, f||X->X, X||X->X, X||!X->t</summary>
		public virtual Db4objects.Db4o.Nativequery.Expr.IExpression Or(Db4objects.Db4o.Nativequery.Expr.IExpression
			 left, Db4objects.Db4o.Nativequery.Expr.IExpression right)
		{
			if (left.Equals(Db4objects.Db4o.Nativequery.Expr.BoolConstExpression.TRUE) || right
				.Equals(Db4objects.Db4o.Nativequery.Expr.BoolConstExpression.TRUE))
			{
				return Db4objects.Db4o.Nativequery.Expr.BoolConstExpression.TRUE;
			}
			if (left.Equals(Db4objects.Db4o.Nativequery.Expr.BoolConstExpression.FALSE))
			{
				return right;
			}
			if (right.Equals(Db4objects.Db4o.Nativequery.Expr.BoolConstExpression.FALSE))
			{
				return left;
			}
			if (left.Equals(right))
			{
				return left;
			}
			if (Negatives(left, right))
			{
				return Db4objects.Db4o.Nativequery.Expr.BoolConstExpression.TRUE;
			}
			return new Db4objects.Db4o.Nativequery.Expr.OrExpression(left, right);
		}

		/// <summary>Optimizations: static bool roots</summary>
		public virtual Db4objects.Db4o.Nativequery.Expr.BoolConstExpression Constant(bool
			 value)
		{
			return Db4objects.Db4o.Nativequery.Expr.BoolConstExpression.Expr(value);
		}

		public virtual Db4objects.Db4o.Nativequery.Expr.IExpression IfThenElse(Db4objects.Db4o.Nativequery.Expr.IExpression
			 cond, Db4objects.Db4o.Nativequery.Expr.IExpression truePath, Db4objects.Db4o.Nativequery.Expr.IExpression
			 falsePath)
		{
			Db4objects.Db4o.Nativequery.Expr.IExpression expr = CheckBoolean(cond, truePath, 
				falsePath);
			if (expr != null)
			{
				return expr;
			}
			return Or(And(cond, truePath), And(Not(cond), falsePath));
		}

		private Db4objects.Db4o.Nativequery.Expr.IExpression CheckBoolean(Db4objects.Db4o.Nativequery.Expr.IExpression
			 cmp, Db4objects.Db4o.Nativequery.Expr.IExpression trueExpr, Db4objects.Db4o.Nativequery.Expr.IExpression
			 falseExpr)
		{
			if (cmp is Db4objects.Db4o.Nativequery.Expr.BoolConstExpression)
			{
				return null;
			}
			if (trueExpr is Db4objects.Db4o.Nativequery.Expr.BoolConstExpression)
			{
				bool leftNegative = trueExpr.Equals(Db4objects.Db4o.Nativequery.Expr.BoolConstExpression
					.FALSE);
				if (!leftNegative)
				{
					return Or(cmp, falseExpr);
				}
				else
				{
					return And(Not(cmp), falseExpr);
				}
			}
			if (falseExpr is Db4objects.Db4o.Nativequery.Expr.BoolConstExpression)
			{
				bool rightNegative = falseExpr.Equals(Db4objects.Db4o.Nativequery.Expr.BoolConstExpression
					.FALSE);
				if (!rightNegative)
				{
					return And(cmp, trueExpr);
				}
				else
				{
					return Or(Not(cmp), falseExpr);
				}
			}
			if (cmp is Db4objects.Db4o.Nativequery.Expr.NotExpression)
			{
				cmp = ((Db4objects.Db4o.Nativequery.Expr.NotExpression)cmp).Expr();
				Db4objects.Db4o.Nativequery.Expr.IExpression swap = trueExpr;
				trueExpr = falseExpr;
				falseExpr = swap;
			}
			if (trueExpr is Db4objects.Db4o.Nativequery.Expr.OrExpression)
			{
				Db4objects.Db4o.Nativequery.Expr.OrExpression orExpr = (Db4objects.Db4o.Nativequery.Expr.OrExpression
					)trueExpr;
				Db4objects.Db4o.Nativequery.Expr.IExpression orLeft = orExpr.Left();
				Db4objects.Db4o.Nativequery.Expr.IExpression orRight = orExpr.Right();
				if (falseExpr.Equals(orRight))
				{
					Db4objects.Db4o.Nativequery.Expr.IExpression swap = orRight;
					orRight = orLeft;
					orLeft = swap;
				}
				if (falseExpr.Equals(orLeft))
				{
					return Or(orLeft, And(cmp, orRight));
				}
			}
			if (falseExpr is Db4objects.Db4o.Nativequery.Expr.AndExpression)
			{
				Db4objects.Db4o.Nativequery.Expr.AndExpression andExpr = (Db4objects.Db4o.Nativequery.Expr.AndExpression
					)falseExpr;
				Db4objects.Db4o.Nativequery.Expr.IExpression andLeft = andExpr.Left();
				Db4objects.Db4o.Nativequery.Expr.IExpression andRight = andExpr.Right();
				if (trueExpr.Equals(andRight))
				{
					Db4objects.Db4o.Nativequery.Expr.IExpression swap = andRight;
					andRight = andLeft;
					andLeft = swap;
				}
				if (trueExpr.Equals(andLeft))
				{
					return And(andLeft, Or(cmp, andRight));
				}
			}
			return null;
		}

		private bool Negatives(Db4objects.Db4o.Nativequery.Expr.IExpression left, Db4objects.Db4o.Nativequery.Expr.IExpression
			 right)
		{
			return NegativeOf(left, right) || NegativeOf(right, left);
		}

		private bool NegativeOf(Db4objects.Db4o.Nativequery.Expr.IExpression right, Db4objects.Db4o.Nativequery.Expr.IExpression
			 left)
		{
			return (right is Db4objects.Db4o.Nativequery.Expr.NotExpression) && ((Db4objects.Db4o.Nativequery.Expr.NotExpression
				)right).Expr().Equals(left);
		}
	}
}
