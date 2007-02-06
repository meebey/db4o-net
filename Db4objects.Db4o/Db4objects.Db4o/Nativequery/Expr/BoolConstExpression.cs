namespace Db4objects.Db4o.Nativequery.Expr
{
	public class BoolConstExpression : Db4objects.Db4o.Nativequery.Expr.IExpression
	{
		public static readonly Db4objects.Db4o.Nativequery.Expr.BoolConstExpression TRUE = 
			new Db4objects.Db4o.Nativequery.Expr.BoolConstExpression(true);

		public static readonly Db4objects.Db4o.Nativequery.Expr.BoolConstExpression FALSE
			 = new Db4objects.Db4o.Nativequery.Expr.BoolConstExpression(false);

		private bool _value;

		private BoolConstExpression(bool value)
		{
			this._value = value;
		}

		public virtual bool Value()
		{
			return _value;
		}

		public override string ToString()
		{
			return _value.ToString();
		}

		public static Db4objects.Db4o.Nativequery.Expr.BoolConstExpression Expr(bool value
			)
		{
			return (value ? TRUE : FALSE);
		}

		public virtual void Accept(Db4objects.Db4o.Nativequery.Expr.IExpressionVisitor visitor
			)
		{
			visitor.Visit(this);
		}

		public virtual Db4objects.Db4o.Nativequery.Expr.IExpression Negate()
		{
			return (_value ? FALSE : TRUE);
		}
	}
}
