using Db4objects.Db4o.Nativequery.Expr.Cmp;

namespace Db4objects.Db4o.Nativequery.Expr.Cmp.Field
{
	public class CandidateFieldRoot : ComparisonOperandRoot
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

		public override void Accept(IComparisonOperandVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
