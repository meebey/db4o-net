using Db4objects.Db4o.Nativequery.Expr.Cmp;

namespace Db4objects.Db4o.Nativequery.Expr.Cmp
{
	public abstract class ComparisonOperandRoot : IComparisonOperandAnchor
	{
		public virtual IComparisonOperandAnchor Parent()
		{
			return null;
		}

		public IComparisonOperandAnchor Root()
		{
			return this;
		}

		public abstract void Accept(IComparisonOperandVisitor arg1);
	}
}
