namespace Db4objects.Db4o.Nativequery.Optimization
{
	public class SODAQueryBuilder
	{
		private class SODAQueryVisitor : Db4objects.Db4o.Nativequery.Expr.IExpressionVisitor
		{
			private object _predicate;

			private Db4objects.Db4o.Query.IQuery _query;

			private Db4objects.Db4o.Query.IConstraint _constraint;

			internal SODAQueryVisitor(Db4objects.Db4o.Query.IQuery query, object predicate)
			{
				_query = query;
				_predicate = predicate;
			}

			public virtual void Visit(Db4objects.Db4o.Nativequery.Expr.AndExpression expression
				)
			{
				expression.Left().Accept(this);
				Db4objects.Db4o.Query.IConstraint left = _constraint;
				expression.Right().Accept(this);
				left.And(_constraint);
				_constraint = left;
			}

			public virtual void Visit(Db4objects.Db4o.Nativequery.Expr.BoolConstExpression expression
				)
			{
			}

			public virtual void Visit(Db4objects.Db4o.Nativequery.Expr.OrExpression expression
				)
			{
				expression.Left().Accept(this);
				Db4objects.Db4o.Query.IConstraint left = _constraint;
				expression.Right().Accept(this);
				left.Or(_constraint);
				_constraint = left;
			}

			public virtual void Visit(Db4objects.Db4o.Nativequery.Expr.ComparisonExpression expression
				)
			{
				Db4objects.Db4o.Query.IQuery subQuery = _query;
				System.Collections.IEnumerator fieldNameIterator = FieldNames(expression.Left());
				while (fieldNameIterator.MoveNext())
				{
					subQuery = subQuery.Descend((string)fieldNameIterator.Current);
				}
				Db4objects.Db4o.Nativequery.Optimization.ComparisonQueryGeneratingVisitor visitor
					 = new Db4objects.Db4o.Nativequery.Optimization.ComparisonQueryGeneratingVisitor
					(_predicate);
				expression.Right().Accept(visitor);
				_constraint = subQuery.Constrain(visitor.Value());
				if (!expression.Op().Equals(Db4objects.Db4o.Nativequery.Expr.Cmp.ComparisonOperator
					.EQUALS))
				{
					if (expression.Op().Equals(Db4objects.Db4o.Nativequery.Expr.Cmp.ComparisonOperator
						.GREATER))
					{
						_constraint.Greater();
					}
					else
					{
						if (expression.Op().Equals(Db4objects.Db4o.Nativequery.Expr.Cmp.ComparisonOperator
							.SMALLER))
						{
							_constraint.Smaller();
						}
						else
						{
							if (expression.Op().Equals(Db4objects.Db4o.Nativequery.Expr.Cmp.ComparisonOperator
								.CONTAINS))
							{
								_constraint.Contains();
							}
							else
							{
								if (expression.Op().Equals(Db4objects.Db4o.Nativequery.Expr.Cmp.ComparisonOperator
									.STARTSWITH))
								{
									_constraint.StartsWith(true);
								}
								else
								{
									if (expression.Op().Equals(Db4objects.Db4o.Nativequery.Expr.Cmp.ComparisonOperator
										.ENDSWITH))
									{
										_constraint.EndsWith(true);
									}
									else
									{
										throw new System.Exception("Can't handle constraint: " + expression.Op());
									}
								}
							}
						}
					}
				}
			}

			public virtual void Visit(Db4objects.Db4o.Nativequery.Expr.NotExpression expression
				)
			{
				expression.Expr().Accept(this);
				_constraint.Not();
			}

			private System.Collections.IEnumerator FieldNames(Db4objects.Db4o.Nativequery.Expr.Cmp.FieldValue
				 fieldValue)
			{
				Db4objects.Db4o.Foundation.Collection4 coll = new Db4objects.Db4o.Foundation.Collection4
					();
				Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperand curOp = fieldValue;
				while (curOp is Db4objects.Db4o.Nativequery.Expr.Cmp.FieldValue)
				{
					Db4objects.Db4o.Nativequery.Expr.Cmp.FieldValue curField = (Db4objects.Db4o.Nativequery.Expr.Cmp.FieldValue
						)curOp;
					coll.Prepend(curField.FieldName());
					curOp = curField.Parent();
				}
				return coll.GetEnumerator();
			}
		}

		public virtual void OptimizeQuery(Db4objects.Db4o.Nativequery.Expr.IExpression expr
			, Db4objects.Db4o.Query.IQuery query, object predicate)
		{
			expr.Accept(new Db4objects.Db4o.Nativequery.Optimization.SODAQueryBuilder.SODAQueryVisitor
				(query, predicate));
		}
	}
}
