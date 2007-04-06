using Db4objects.Db4o.Nativequery.Expr;
using Db4objects.Db4o.Nativequery.Expr.Cmp;

namespace Db4objects.Db4o.Nativequery.Expr.Build
{
	public class ExpressionBuilder
	{
		/// <summary>Optimizations: !(Bool)->(!Bool), !!X->X, !(X==Bool)->(X==!Bool)</summary>
		public virtual IExpression Not(IExpression expr)
		{
			if (expr.Equals(BoolConstExpression.TRUE))
			{
				return BoolConstExpression.FALSE;
			}
			if (expr.Equals(BoolConstExpression.FALSE))
			{
				return BoolConstExpression.TRUE;
			}
			if (expr is NotExpression)
			{
				return ((NotExpression)expr).Expr();
			}
			if (expr is ComparisonExpression)
			{
				ComparisonExpression cmpExpr = (ComparisonExpression)expr;
				if (cmpExpr.Right() is ConstValue)
				{
					ConstValue rightConst = (ConstValue)cmpExpr.Right();
					if (rightConst.Value() is bool)
					{
						bool boolVal = (bool)rightConst.Value();
						return new ComparisonExpression(cmpExpr.Left(), new ConstValue(!boolVal), cmpExpr
							.Op());
					}
				}
			}
			return new NotExpression(expr);
		}

		/// <summary>Optimizations: f&&X->f, t&&X->X, X&&X->X, X&&!X->f</summary>
		public virtual IExpression And(IExpression left, IExpression right)
		{
			if (left.Equals(BoolConstExpression.FALSE) || right.Equals(BoolConstExpression.FALSE
				))
			{
				return BoolConstExpression.FALSE;
			}
			if (left.Equals(BoolConstExpression.TRUE))
			{
				return right;
			}
			if (right.Equals(BoolConstExpression.TRUE))
			{
				return left;
			}
			if (left.Equals(right))
			{
				return left;
			}
			if (Negatives(left, right))
			{
				return BoolConstExpression.FALSE;
			}
			return new AndExpression(left, right);
		}

		/// <summary>Optimizations: X||t->t, f||X->X, X||X->X, X||!X->t</summary>
		public virtual IExpression Or(IExpression left, IExpression right)
		{
			if (left.Equals(BoolConstExpression.TRUE) || right.Equals(BoolConstExpression.TRUE
				))
			{
				return BoolConstExpression.TRUE;
			}
			if (left.Equals(BoolConstExpression.FALSE))
			{
				return right;
			}
			if (right.Equals(BoolConstExpression.FALSE))
			{
				return left;
			}
			if (left.Equals(right))
			{
				return left;
			}
			if (Negatives(left, right))
			{
				return BoolConstExpression.TRUE;
			}
			return new OrExpression(left, right);
		}

		/// <summary>Optimizations: static bool roots</summary>
		public virtual BoolConstExpression Constant(bool value)
		{
			return BoolConstExpression.Expr(value);
		}

		public virtual IExpression IfThenElse(IExpression cond, IExpression truePath, IExpression
			 falsePath)
		{
			IExpression expr = CheckBoolean(cond, truePath, falsePath);
			if (expr != null)
			{
				return expr;
			}
			return Or(And(cond, truePath), And(Not(cond), falsePath));
		}

		private IExpression CheckBoolean(IExpression cmp, IExpression trueExpr, IExpression
			 falseExpr)
		{
			if (cmp is BoolConstExpression)
			{
				return null;
			}
			if (trueExpr is BoolConstExpression)
			{
				bool leftNegative = trueExpr.Equals(BoolConstExpression.FALSE);
				if (!leftNegative)
				{
					return Or(cmp, falseExpr);
				}
				else
				{
					return And(Not(cmp), falseExpr);
				}
			}
			if (falseExpr is BoolConstExpression)
			{
				bool rightNegative = falseExpr.Equals(BoolConstExpression.FALSE);
				if (!rightNegative)
				{
					return And(cmp, trueExpr);
				}
				else
				{
					return Or(Not(cmp), falseExpr);
				}
			}
			if (cmp is NotExpression)
			{
				cmp = ((NotExpression)cmp).Expr();
				IExpression swap = trueExpr;
				trueExpr = falseExpr;
				falseExpr = swap;
			}
			if (trueExpr is OrExpression)
			{
				OrExpression orExpr = (OrExpression)trueExpr;
				IExpression orLeft = orExpr.Left();
				IExpression orRight = orExpr.Right();
				if (falseExpr.Equals(orRight))
				{
					IExpression swap = orRight;
					orRight = orLeft;
					orLeft = swap;
				}
				if (falseExpr.Equals(orLeft))
				{
					return Or(orLeft, And(cmp, orRight));
				}
			}
			if (falseExpr is AndExpression)
			{
				AndExpression andExpr = (AndExpression)falseExpr;
				IExpression andLeft = andExpr.Left();
				IExpression andRight = andExpr.Right();
				if (trueExpr.Equals(andRight))
				{
					IExpression swap = andRight;
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

		private bool Negatives(IExpression left, IExpression right)
		{
			return NegativeOf(left, right) || NegativeOf(right, left);
		}

		private bool NegativeOf(IExpression right, IExpression left)
		{
			return (right is NotExpression) && ((NotExpression)right).Expr().Equals(left);
		}
	}
}
