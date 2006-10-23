namespace Db4objects.Db4o.Nativequery.Expr
{
	public interface IExpression
	{
		/// <param name="visitor">
		/// must implement the visitor interface required
		/// by the concrete Expression implementation.
		/// </param>
		void Accept(Db4objects.Db4o.Nativequery.Expr.IExpressionVisitor visitor);
	}
}
