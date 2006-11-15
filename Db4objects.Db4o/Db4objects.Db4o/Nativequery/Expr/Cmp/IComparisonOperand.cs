namespace Db4objects.Db4o.Nativequery.Expr.Cmp
{
	public interface IComparisonOperand
	{
		void Accept(Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperandVisitor visitor
			);
	}
}
