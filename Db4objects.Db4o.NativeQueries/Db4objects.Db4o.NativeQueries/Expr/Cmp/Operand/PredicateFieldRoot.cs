/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.NativeQueries.Expr.Cmp.Operand;

namespace Db4objects.Db4o.NativeQueries.Expr.Cmp.Operand
{
	public class PredicateFieldRoot : ComparisonOperandRoot
	{
		public static readonly PredicateFieldRoot Instance = new PredicateFieldRoot();

		private PredicateFieldRoot()
		{
		}

		public override string ToString()
		{
			return "PREDICATE";
		}

		public override void Accept(IComparisonOperandVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
