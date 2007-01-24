namespace Db4objects.Db4o.Nativequery.Expr.Cmp
{
	public class ThreeWayComparison
	{
		private Db4objects.Db4o.Nativequery.Expr.Cmp.FieldValue _left;

		private Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperand _right;

		private bool _swapped;

		public ThreeWayComparison(Db4objects.Db4o.Nativequery.Expr.Cmp.FieldValue left, Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperand
			 right, bool swapped)
		{
			this._left = left;
			this._right = right;
			_swapped = swapped;
		}

		public virtual Db4objects.Db4o.Nativequery.Expr.Cmp.FieldValue Left()
		{
			return _left;
		}

		public virtual Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperand Right()
		{
			return _right;
		}

		public virtual bool Swapped()
		{
			return _swapped;
		}
	}
}
