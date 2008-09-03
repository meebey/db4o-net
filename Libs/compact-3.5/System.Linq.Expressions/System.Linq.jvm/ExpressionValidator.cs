using System;
using System.Linq.Expressions;

namespace System.Linq.jvm {

	class ExpressionValidator : ExpressionVisitor
	{
		LambdaExpression lambda;

		public ExpressionValidator (LambdaExpression lambda)
		{
			this.lambda = lambda;
		}

		protected override void VisitParameter (ParameterExpression parameter)
		{
			foreach (var param in lambda.Parameters) {
				if (param.Name == parameter.Name && param != parameter)
					throw new InvalidOperationException ("Lambda Parameter not in scope");
			}
		}

		public void Validate()
		{
			Visit (lambda);
		}
    }
}
