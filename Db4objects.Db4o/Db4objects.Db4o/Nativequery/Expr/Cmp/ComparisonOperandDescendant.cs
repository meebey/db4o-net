using Db4objects.Db4o.Nativequery.Expr.Cmp;

namespace Db4objects.Db4o.Nativequery.Expr.Cmp
{
	public abstract class ComparisonOperandDescendant : IComparisonOperandAnchor
	{
		private IComparisonOperandAnchor _parent;

		protected ComparisonOperandDescendant(IComparisonOperandAnchor _parent)
		{
			this._parent = _parent;
		}

		public IComparisonOperandAnchor Parent()
		{
			return _parent;
		}

		public IComparisonOperandAnchor Root()
		{
			return _parent.Root();
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			Db4objects.Db4o.Nativequery.Expr.Cmp.ComparisonOperandDescendant casted = (Db4objects.Db4o.Nativequery.Expr.Cmp.ComparisonOperandDescendant
				)obj;
			return _parent.Equals(casted._parent);
		}

		public override int GetHashCode()
		{
			return _parent.GetHashCode();
		}

		public override string ToString()
		{
			return _parent.ToString();
		}

		public abstract void Accept(IComparisonOperandVisitor arg1);
	}
}
