namespace Db4objects.Db4o.Nativequery.Expr.Cmp.Field
{
	public class CandidateFieldRoot : Db4objects.Db4o.Nativequery.Expr.Cmp.ComparisonOperandRoot
	{
		public static readonly Db4objects.Db4o.Nativequery.Expr.Cmp.Field.CandidateFieldRoot
			 INSTANCE = new Db4objects.Db4o.Nativequery.Expr.Cmp.Field.CandidateFieldRoot();

		private CandidateFieldRoot()
		{
		}

		public override string ToString()
		{
			return "CANDIDATE";
		}

		public override void Accept(Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperandVisitor
			 visitor)
		{
			visitor.Visit(this);
		}
	}
}
