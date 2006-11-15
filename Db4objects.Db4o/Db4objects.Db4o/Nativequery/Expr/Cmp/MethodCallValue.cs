namespace Db4objects.Db4o.Nativequery.Expr.Cmp
{
	public class MethodCallValue : Db4objects.Db4o.Nativequery.Expr.Cmp.ComparisonOperandDescendant
	{
		private string _methodName;

		private System.Type[] _paramTypes;

		private Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperand[] _args;

		public MethodCallValue(Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperandAnchor
			 parent, string name, System.Type[] paramTypes, Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperand[]
			 args) : base(parent)
		{
			_methodName = name;
			_paramTypes = paramTypes;
			_args = args;
		}

		public override void Accept(Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperandVisitor
			 visitor)
		{
			visitor.Visit(this);
		}

		public virtual string MethodName()
		{
			return _methodName;
		}

		public virtual System.Type[] ParamTypes()
		{
			return _paramTypes;
		}

		public virtual Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperand[] Args()
		{
			return _args;
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			Db4objects.Db4o.Nativequery.Expr.Cmp.MethodCallValue casted = (Db4objects.Db4o.Nativequery.Expr.Cmp.MethodCallValue
				)obj;
			return _methodName.Equals(casted._methodName) && ArrayCmp(_paramTypes, casted._paramTypes
				) && ArrayCmp(_args, casted._args);
		}

		public override int GetHashCode()
		{
			int hc = base.GetHashCode();
			hc *= 29 + _methodName.GetHashCode();
			hc *= 29 + _paramTypes.GetHashCode();
			hc *= 29 + _args.GetHashCode();
			return hc;
		}

		public override string ToString()
		{
			string str = base.ToString() + "." + _methodName + "(";
			for (int paramIdx = 0; paramIdx < _paramTypes.Length; paramIdx++)
			{
				if (paramIdx > 0)
				{
					str += ",";
				}
				str += _paramTypes[paramIdx] + ":" + _args[paramIdx];
			}
			str += ")";
			return str;
		}

		private bool ArrayCmp(object[] a, object[] b)
		{
			if (a.Length != b.Length)
			{
				return false;
			}
			for (int paramIdx = 0; paramIdx < a.Length; paramIdx++)
			{
				if (!a[paramIdx].Equals(b[paramIdx]))
				{
					return false;
				}
			}
			return true;
		}
	}
}
