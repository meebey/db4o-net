using Db4objects.Db4o.Nativequery.Expr.Cmp;

namespace Db4objects.Db4o.Nativequery.Expr.Cmp
{
	public interface IComparisonOperand
	{
		void Accept(IComparisonOperandVisitor visitor);
	}
}
