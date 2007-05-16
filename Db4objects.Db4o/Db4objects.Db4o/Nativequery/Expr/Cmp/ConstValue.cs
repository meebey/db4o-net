/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Nativequery.Expr.Cmp;

namespace Db4objects.Db4o.Nativequery.Expr.Cmp
{
	public class ConstValue : IComparisonOperand
	{
		private object _value;

		public ConstValue(object value)
		{
			this._value = value;
		}

		public virtual object Value()
		{
			return _value;
		}

		public virtual void Value(object value)
		{
			_value = value;
		}

		public override string ToString()
		{
			if (_value == null)
			{
				return "null";
			}
			if (_value is string)
			{
				return "\"" + _value + "\"";
			}
			return _value.ToString();
		}

		public override bool Equals(object other)
		{
			if (this == other)
			{
				return true;
			}
			if (other == null || GetType() != other.GetType())
			{
				return false;
			}
			object otherValue = ((Db4objects.Db4o.Nativequery.Expr.Cmp.ConstValue)other)._value;
			if (otherValue == _value)
			{
				return true;
			}
			if (otherValue == null || _value == null)
			{
				return false;
			}
			return _value.Equals(otherValue);
		}

		public override int GetHashCode()
		{
			return _value.GetHashCode();
		}

		public virtual void Accept(IComparisonOperandVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
