/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Nativequery.Expr.Cmp;

namespace Db4objects.Db4o.Nativequery.Expr.Cmp
{
	public class ArrayAccessValue : ComparisonOperandDescendant
	{
		private IComparisonOperand _index;

		public ArrayAccessValue(IComparisonOperandAnchor parent, IComparisonOperand index
			) : base(parent)
		{
			_index = index;
		}

		public override void Accept(IComparisonOperandVisitor visitor)
		{
			visitor.Visit(this);
		}

		public virtual IComparisonOperand Index()
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
