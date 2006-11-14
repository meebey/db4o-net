namespace Db4objects.Db4o.Nativequery.Expr
{
	public class TraversingExpressionVisitor : Db4objects.Db4o.Nativequery.Expr.IExpressionVisitor
		, Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperandVisitor
	{
		public virtual void Visit(Db4objects.Db4o.Nativequery.Expr.AndExpression expression
			)
		{
			expression.Left().Accept(this);
			expression.Right().Accept(this);
		}

		public virtual void Visit(Db4objects.Db4o.Nativequery.Expr.BoolConstExpression expression
			)
		{
		}

		public virtual void Visit(Db4objects.Db4o.Nativequery.Expr.OrExpression expression
			)
		{
			expression.Left().Accept(this);
			expression.Right().Accept(this);
		}

		public virtual void Visit(Db4objects.Db4o.Nativequery.Expr.ComparisonExpression expression
			)
		{
			expression.Left().Accept(this);
			expression.Right().Accept(this);
		}

		public virtual void Visit(Db4objects.Db4o.Nativequery.Expr.NotExpression expression
			)
		{
			expression.Expr().Accept(this);
		}

		public virtual void Visit(Db4objects.Db4o.Nativequery.Expr.Cmp.ArithmeticExpression
			 operand)
		{
			operand.Left().Accept(this);
			operand.Right().Accept(this);
		}

		public virtual void Visit(Db4objects.Db4o.Nativequery.Expr.Cmp.ConstValue operand
			)
		{
		}

		public virtual void Visit(Db4objects.Db4o.Nativequery.Expr.Cmp.FieldValue operand
			)
		{
			operand.Parent().Accept(this);
		}

		public virtual void Visit(Db4objects.Db4o.Nativequery.Expr.Cmp.Field.CandidateFieldRoot
			 root)
		{
		}

		public virtual void Visit(Db4objects.Db4o.Nativequery.Expr.Cmp.Field.PredicateFieldRoot
			 root)
		{
		}

		public virtual void Visit(Db4objects.Db4o.Nativequery.Expr.Cmp.Field.StaticFieldRoot
			 root)
		{
		}

		public virtual void Visit(Db4objects.Db4o.Nativequery.Expr.Cmp.ArrayAccessValue operand
			)
		{
			operand.Parent().Accept(this);
			operand.Index().Accept(this);
		}

		public virtual void Visit(Db4objects.Db4o.Nativequery.Expr.Cmp.MethodCallValue value
			)
		{
			value.Parent().Accept(this);
			VisitArgs(value);
		}

		protected virtual void VisitArgs(Db4objects.Db4o.Nativequery.Expr.Cmp.MethodCallValue
			 value)
		{
			Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperand[] args = value.Args();
			for (int i = 0; i < args.Length; ++i)
			{
				args[i].Accept(this);
			}
		}
	}
}
