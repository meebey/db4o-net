/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Nativequery.Expr.Cmp;

namespace Db4objects.Db4o.Nativequery.Expr.Cmp
{
	public interface IComparisonOperandAnchor : IComparisonOperand
	{
		IComparisonOperandAnchor Parent();

		IComparisonOperandAnchor Root();
	}
}
