/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Instrumentation.Api;
using Db4objects.Db4o.NativeQueries.Expr.Cmp.Operand;

namespace Db4objects.Db4o.NativeQueries.Expr.Cmp.Operand
{
	public class MethodCallValue : ComparisonOperandDescendant
	{
		private readonly IMethodRef _method;

		private readonly IComparisonOperand[] _args;

		public MethodCallValue(IComparisonOperandAnchor parent, IMethodRef method, IComparisonOperand
			[] args) : base(parent)
		{
			_method = method;
			_args = args;
		}

		public override void Accept(IComparisonOperandVisitor visitor)
		{
			visitor.Visit(this);
		}

		public virtual IComparisonOperand[] Args
		{
			get
			{
				return _args;
			}
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			Db4objects.Db4o.NativeQueries.Expr.Cmp.Operand.MethodCallValue casted = (Db4objects.Db4o.NativeQueries.Expr.Cmp.Operand.MethodCallValue
				)obj;
			return _method.Equals(casted._method);
		}

		public override int GetHashCode()
		{
			int hc = base.GetHashCode();
			hc *= 29 + _method.GetHashCode();
			hc *= 29 + _args.GetHashCode();
			return hc;
		}

		public override string ToString()
		{
			return base.ToString() + "." + _method.Name + Iterators.Join(Iterators.Iterate(_args
				), "(", ")", ", ");
		}

		public virtual IMethodRef Method
		{
			get
			{
				return _method;
			}
		}
	}
}
