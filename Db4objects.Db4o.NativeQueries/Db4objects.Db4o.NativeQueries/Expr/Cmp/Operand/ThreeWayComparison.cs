/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.NativeQueries.Expr.Cmp.Operand;

namespace Db4objects.Db4o.NativeQueries.Expr.Cmp.Operand
{
	public class ThreeWayComparison
	{
		private FieldValue _left;

		private IComparisonOperand _right;

		private bool _swapped;

		public ThreeWayComparison(FieldValue left, IComparisonOperand right, bool swapped
			)
		{
			this._left = left;
			this._right = right;
			_swapped = swapped;
		}

		public virtual FieldValue Left()
		{
			return _left;
		}

		public virtual IComparisonOperand Right()
		{
			return _right;
		}

		public virtual bool Swapped()
		{
			return _swapped;
		}
	}
}
