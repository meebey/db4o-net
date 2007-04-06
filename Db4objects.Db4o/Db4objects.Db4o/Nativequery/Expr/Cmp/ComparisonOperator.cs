namespace Db4objects.Db4o.Nativequery.Expr.Cmp
{
	public sealed class ComparisonOperator
	{
		public const int EQUALS_ID = 0;

		public const int SMALLER_ID = 1;

		public const int GREATER_ID = 2;

		public const int CONTAINS_ID = 3;

		public const int STARTSWITH_ID = 4;

		public const int ENDSWITH_ID = 5;

		public static readonly Db4objects.Db4o.Nativequery.Expr.Cmp.ComparisonOperator EQUALS
			 = new Db4objects.Db4o.Nativequery.Expr.Cmp.ComparisonOperator(EQUALS_ID, "==", 
			true);

		public static readonly Db4objects.Db4o.Nativequery.Expr.Cmp.ComparisonOperator SMALLER
			 = new Db4objects.Db4o.Nativequery.Expr.Cmp.ComparisonOperator(SMALLER_ID, "<", 
			false);

		public static readonly Db4objects.Db4o.Nativequery.Expr.Cmp.ComparisonOperator GREATER
			 = new Db4objects.Db4o.Nativequery.Expr.Cmp.ComparisonOperator(GREATER_ID, ">", 
			false);

		public static readonly Db4objects.Db4o.Nativequery.Expr.Cmp.ComparisonOperator CONTAINS
			 = new Db4objects.Db4o.Nativequery.Expr.Cmp.ComparisonOperator(CONTAINS_ID, "<CONTAINS>"
			, false);

		public static readonly Db4objects.Db4o.Nativequery.Expr.Cmp.ComparisonOperator STARTSWITH
			 = new Db4objects.Db4o.Nativequery.Expr.Cmp.ComparisonOperator(STARTSWITH_ID, "<STARTSWITH>"
			, false);

		public static readonly Db4objects.Db4o.Nativequery.Expr.Cmp.ComparisonOperator ENDSWITH
			 = new Db4objects.Db4o.Nativequery.Expr.Cmp.ComparisonOperator(ENDSWITH_ID, "<ENDSWITH>"
			, false);

		private int _id;

		private string _op;

		private bool _symmetric;

		private ComparisonOperator(int id, string op, bool symmetric)
		{
			_id = id;
			_op = op;
			_symmetric = symmetric;
		}

		public int Id()
		{
			return _id;
		}

		public override string ToString()
		{
			return _op;
		}

		public bool IsSymmetric()
		{
			return _symmetric;
		}
	}
}
