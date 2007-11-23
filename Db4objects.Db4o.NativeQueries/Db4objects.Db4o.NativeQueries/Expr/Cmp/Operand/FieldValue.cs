/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.NativeQueries.Expr.Cmp.Operand;

namespace Db4objects.Db4o.NativeQueries.Expr.Cmp.Operand
{
	public class FieldValue : ComparisonOperandDescendant
	{
		private string _fieldName;

		private object _tag;

		public FieldValue(IComparisonOperandAnchor root, string name) : this(root, name, 
			null)
		{
		}

		public FieldValue(IComparisonOperandAnchor root, string name, object tag) : base(
			root)
		{
			_fieldName = name;
			_tag = tag;
		}

		public virtual string FieldName()
		{
			return _fieldName;
		}

		public override bool Equals(object other)
		{
			if (!base.Equals(other))
			{
				return false;
			}
			Db4objects.Db4o.NativeQueries.Expr.Cmp.Operand.FieldValue casted = (Db4objects.Db4o.NativeQueries.Expr.Cmp.Operand.FieldValue
				)other;
			if (_tag == null)
			{
				if (casted._tag != null)
				{
					return false;
				}
			}
			else
			{
				if (!_tag.Equals(casted._tag))
				{
					return false;
				}
			}
			return _fieldName.Equals(casted._fieldName);
		}

		public override int GetHashCode()
		{
			int hash = base.GetHashCode() * 29 + _fieldName.GetHashCode();
			if (_tag != null)
			{
				hash *= 29 + _tag.GetHashCode();
			}
			return hash;
		}

		public override string ToString()
		{
			return base.ToString() + "." + _fieldName;
		}

		public override void Accept(IComparisonOperandVisitor visitor)
		{
			visitor.Visit(this);
		}

		/// <summary>Code analysis specific information.</summary>
		/// <remarks>
		/// Code analysis specific information.
		/// This is used in the .net side to preserve Mono.Cecil references
		/// for instance.
		/// </remarks>
		public virtual object Tag()
		{
			return _tag;
		}

		public virtual void Tag(object value)
		{
			_tag = value;
		}
	}
}
