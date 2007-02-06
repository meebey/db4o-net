namespace Db4objects.Db4o.Nativequery.Expr.Cmp
{
	public class FieldValue : Db4objects.Db4o.Nativequery.Expr.Cmp.ComparisonOperandDescendant
	{
		private string _fieldName;

		private object _tag;

		public FieldValue(Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperandAnchor root
			, string name) : this(root, name, null)
		{
		}

		public FieldValue(Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperandAnchor root
			, string name, object tag) : base(root)
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
			Db4objects.Db4o.Nativequery.Expr.Cmp.FieldValue casted = (Db4objects.Db4o.Nativequery.Expr.Cmp.FieldValue
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

		public override void Accept(Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperandVisitor
			 visitor)
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
