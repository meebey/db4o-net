namespace Db4objects.Db4o.Nativequery.Expr.Cmp
{
	public class ArrayAccessValue : Db4objects.Db4o.Nativequery.Expr.Cmp.ComparisonOperandDescendant
	{
		private Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperand _index;

		public ArrayAccessValue(Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperandAnchor
			 parent, Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperand index) : base(parent
			)
		{
			_index = index;
		}

		public override void Accept(Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperandVisitor
			 visitor)
		{
			visitor.Visit(this);
		}

		public virtual Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperand Index()
		{
			return _index;
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			Db4objects.Db4o.Nativequery.Expr.Cmp.ArrayAccessValue casted = (Db4objects.Db4o.Nativequery.Expr.Cmp.ArrayAccessValue
				)obj;
			return _index.Equals(casted._index);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() * 29 + _index.GetHashCode();
		}

		public override string ToString()
		{
			return base.ToString() + "[" + _index + "]";
		}
	}
}
