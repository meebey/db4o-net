/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Linq.Expressions
{
	public class SubtreeEvaluator : ExpressionTransformer
	{
		private HashSet<Expression> _candidates;

		private SubtreeEvaluator(HashSet<Expression> candidates)
		{
			_candidates = candidates;
		}

		public static Expression Evaluate(Expression expression)
		{
			var nominator = new Nominator(expression, exp => exp.NodeType != ExpressionType.Parameter);

			return new SubtreeEvaluator(nominator.Candidates).Visit(expression);
		}

		protected override Expression Visit(Expression expression)
		{
			if (expression == null) return null;
			if (_candidates.Contains(expression)) return EvaluateCandidate(expression);

			return base.Visit(expression);
		}

		private Expression EvaluateCandidate(Expression expression)
		{
			if (expression.NodeType == ExpressionType.Constant) return expression;

			var evaluator = Expression.Lambda(expression).Compile();
			return Expression.Constant(evaluator.DynamicInvoke(null), expression.Type);
		}

		class Nominator : ExpressionTransformer
		{
			Func<Expression, bool> _predicate;
			HashSet<Expression> _candidates = new HashSet<Expression>();
			bool cannotBeEvaluated;

			public HashSet<Expression> Candidates
			{
				get { return _candidates; }
			}

			public Nominator(Expression expression, Func<Expression, bool> predicate)
			{
				_predicate = predicate;

				Visit(expression);
			}

			private void AddCandidate(Expression expression)
			{
				_candidates.Add(expression);
			}

			// TODO: refactor
			protected override Expression Visit(Expression expression)
			{
				if (expression == null) return null;

				bool saveCannotBeEvaluated = cannotBeEvaluated;
				cannotBeEvaluated = false;

				base.Visit(expression);

				if (cannotBeEvaluated) return expression;

				if (_predicate(expression))
				{
					AddCandidate(expression);
				}
				else
				{
					cannotBeEvaluated = true;
				}

				cannotBeEvaluated |= saveCannotBeEvaluated;

				return expression;
			}
		}
	}
}
