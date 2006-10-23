namespace Db4objects.Db4o.Nativequery.Expr.Cmp
{
	public interface IComparisonOperandAnchor : Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperand
	{
		Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperandAnchor Parent();

		Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperandAnchor Root();
	}
}
