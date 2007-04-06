using Db4objects.Db4o.Nativequery.Expr.Cmp;

namespace Db4objects.Db4o.Nativequery.Expr.Cmp
{
	public interface IComparisonOperandAnchor : IComparisonOperand
	{
		IComparisonOperandAnchor Parent();

		IComparisonOperandAnchor Root();
	}
}
