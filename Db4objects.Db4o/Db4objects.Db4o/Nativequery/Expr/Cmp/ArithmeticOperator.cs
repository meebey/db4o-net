namespace Db4objects.Db4o.Nativequery.Expr.Cmp
{
	public sealed class ArithmeticOperator
	{
		public const int ADD_ID = 0;

		public const int SUBTRACT_ID = 1;

		public const int MULTIPLY_ID = 2;

		public const int DIVIDE_ID = 3;

		public static readonly Db4objects.Db4o.Nativequery.Expr.Cmp.ArithmeticOperator ADD
			 = new Db4objects.Db4o.Nativequery.Expr.Cmp.ArithmeticOperator(ADD_ID, "+");

		public static readonly Db4objects.Db4o.Nativequery.Expr.Cmp.ArithmeticOperator SUBTRACT
			 = new Db4objects.Db4o.Nativequery.Expr.Cmp.ArithmeticOperator(SUBTRACT_ID, "-");

		public static readonly Db4objects.Db4o.Nativequery.Expr.Cmp.ArithmeticOperator MULTIPLY
			 = new Db4objects.Db4o.Nativequery.Expr.Cmp.ArithmeticOperator(MULTIPLY_ID, "*");

		public static readonly Db4objects.Db4o.Nativequery.Expr.Cmp.ArithmeticOperator DIVIDE
			 = new Db4objects.Db4o.Nativequery.Expr.Cmp.ArithmeticOperator(DIVIDE_ID, "/");

		private string _op;

		private int _id;

		private ArithmeticOperator(int id, string op)
		{
			_id = id;
			_op = op;
		}

		public int Id()
		{
			return _id;
		}

		public override string ToString()
		{
			return _op;
		}
	}
}
