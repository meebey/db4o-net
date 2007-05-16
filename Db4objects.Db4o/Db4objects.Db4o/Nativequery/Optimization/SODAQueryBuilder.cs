/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Nativequery.Expr;
using Db4objects.Db4o.Nativequery.Expr.Cmp;
using Db4objects.Db4o.Nativequery.Optimization;
using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Nativequery.Optimization
{
	public class SODAQueryBuilder
	{
		private class SODAQueryVisitor : IExpressionVisitor
		{
			private object _predicate;

			private IQuery _query;

			private IConstraint _constraint;

			internal SODAQueryVisitor(IQuery query, object predicate)
			{
				_query = query;
				_predicate = predicate;
			}

			public virtual void Visit(AndExpression expression)
			{
				expression.Left().Accept(this);
				IConstraint left = _constraint;
				expression.Right().Accept(this);
				left.And(_constraint);
				_constraint = left;
			}

			public virtual void Visit(BoolConstExpression expression)
			{
			}

			public virtual void Visit(OrExpression expression)
			{
				expression.Left().Accept(this);
				IConstraint left = _constraint;
				expression.Right().Accept(this);
				left.Or(_constraint);
				_constraint = left;
			}

			public virtual void Visit(ComparisonExpression expression)
			{
				IQuery subQuery = _query;
				IEnumerator fieldNameIterator = FieldNames(expression.Left());
				while (fieldNameIterator.MoveNext())
				{
					subQuery = subQuery.Descend((string)fieldNameIterator.Current);
				}
				ComparisonQueryGeneratingVisitor visitor = new ComparisonQueryGeneratingVisitor(_predicate
					);
				expression.Right().Accept(visitor);
				_constraint = subQuery.Constrain(visitor.Value());
				ComparisonOperator op = expression.Op();
				if (op.Equals(ComparisonOperator.EQUALS))
				{
					return;
				}
				if (op.Equals(ComparisonOperator.IDENTITY))
				{
					_constraint.Identity();
					return;
				}
				if (op.Equals(ComparisonOperator.GREATER))
				{
					_constraint.Greater();
					return;
				}
				if (op.Equals(ComparisonOperator.SMALLER))
				{
					_constraint.Smaller();
					return;
				}
				if (op.Equals(ComparisonOperator.CONTAINS))
				{
					_constraint.Contains();
					return;
				}
				if (op.Equals(ComparisonOperator.STARTSWITH))
				{
					_constraint.StartsWith(true);
					return;
				}
				if (op.Equals(ComparisonOperator.ENDSWITH))
				{
					_constraint.EndsWith(true);
					return;
				}
				throw new Exception("Can't handle constraint: " + op);
			}

			public virtual void Visit(NotExpression expression)
			{
				expression.Expr().Accept(this);
				_constraint.Not();
			}

			private IEnumerator FieldNames(FieldValue fieldValue)
			{
				Collection4 coll = new Collection4();
				IComparisonOperand curOp = fieldValue;
				while (curOp is FieldValue)
				{
					FieldValue curField = (FieldValue)curOp;
					coll.Prepend(curField.FieldName());
					curOp = curField.Parent();
				}
				return coll.GetEnumerator();
			}
		}

		public virtual void OptimizeQuery(IExpression expr, IQuery query, object predicate
			)
		{
			expr.Accept(new SODAQueryBuilder.SODAQueryVisitor(query, predicate));
		}
	}
}
