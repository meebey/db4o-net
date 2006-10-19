namespace Db4objects.Db4o.Nativequery.Expr.Cmp.Field
{
	public class PredicateFieldRoot : Db4objects.Db4o.Nativequery.Expr.Cmp.ComparisonOperandRoot
	{
		public static readonly Db4objects.Db4o.Nativequery.Expr.Cmp.Field.PredicateFieldRoot
			 INSTANCE = new Db4objects.Db4o.Nativequery.Expr.Cmp.Field.PredicateFieldRoot();

		private PredicateFieldRoot()
		{
		}

		public override string ToString()
		{
			return "PREDICATE";
		}

		public override void Accept(Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperandVisitor
			 visitor)
		{
			visitor.Visit(this);
		}
	}
}
