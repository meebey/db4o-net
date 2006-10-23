namespace Db4objects.Db4o.Nativequery.Expr.Cmp.Field
{
	public class StaticFieldRoot : Db4objects.Db4o.Nativequery.Expr.Cmp.ComparisonOperandRoot
	{
		private string _className;

		public StaticFieldRoot(string className)
		{
			this._className = className;
		}

		public virtual string ClassName()
		{
			return _className;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			Db4objects.Db4o.Nativequery.Expr.Cmp.Field.StaticFieldRoot casted = (Db4objects.Db4o.Nativequery.Expr.Cmp.Field.StaticFieldRoot
				)obj;
			return _className.Equals(casted._className);
		}

		public override int GetHashCode()
		{
			return _className.GetHashCode();
		}

		public override string ToString()
		{
			return _className;
		}

		public override void Accept(Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperandVisitor
			 visitor)
		{
			visitor.Visit(this);
		}
	}
}
