using System;
using Db4objects.Db4o.Nativequery.Expr.Cmp;

namespace Db4objects.Db4o.Nativequery.Expr.Cmp
{
	public class MethodCallValue : ComparisonOperandDescendant
	{
		private string _methodName;

		private Type[] _paramTypes;

		private IComparisonOperand[] _args;

		public MethodCallValue(IComparisonOperandAnchor parent, string name, Type[] paramTypes
			, IComparisonOperand[] args) : base(parent)
		{
			_methodName = name;
			_paramTypes = paramTypes;
			_args = args;
		}

		public override void Accept(IComparisonOperandVisitor visitor)
		{
			visitor.Visit(this);
		}

		public virtual string MethodName()
		{
			return _methodName;
		}

		public virtual Type[] ParamTypes()
		{
			return _paramTypes;
		}

		public virtual IComparisonOperand[] Args()
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
