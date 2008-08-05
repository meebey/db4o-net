using System;
using System.Linq.Expressions;
using Db4objects.Db4o.Linq.Expressions;

namespace Db4objects.Db4o.Linq.Expressions
{
	class ExpressionTreeNormalizer : ExpressionTransformer
	{
		protected override System.Linq.Expressions.Expression VisitBinary(System.Linq.Expressions.BinaryExpression b)
		{
			return NormalizeVisualBasicOperator(b) ?? base.VisitBinary(b);
		}

		private Expression NormalizeVisualBasicOperator(BinaryExpression b)
		{
			var call = b.Left as MethodCallExpression;
			if (call == null) return null;
			if (call.Object != null) return null;
			if (call.Method.DeclaringType.FullName != "Microsoft.VisualBasic.CompilerServices.Operators") return null;

			switch (call.Method.Name)
			{
				case "CompareString":
					{
						switch (b.NodeType)
						{
							case ExpressionType.Equal:
								return ToStringEquals(call);
							case ExpressionType.NotEqual:
								return Expression.Not(ToStringEquals(call));
						}

						return null;
					}
			}
			return null;
		}

		private MethodCallExpression ToStringEquals(MethodCallExpression call)
		{
			var stringEquals = typeof(string).GetMethod("Equals", new[] {typeof(string)});
			return Expression.Call(call.Arguments[0], stringEquals, call.Arguments[1]);
		}

		public Expression Normalize(Expression expression)
		{
			return Visit(expression);
		}
	}
}
