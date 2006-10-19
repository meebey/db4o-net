namespace Db4objects.Db4o.Nativequery.Expr.Cmp
{
	public abstract class ComparisonOperandRoot : Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperandAnchor
	{
		public virtual Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperandAnchor Parent
			()
		{
			return null;
		}

		public Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperandAnchor Root()
		{
			return this;
		}

		public abstract void Accept(Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperandVisitor
			 arg1);
	}
}
