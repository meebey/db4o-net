/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Btree.Algebra;

namespace Db4objects.Db4o.Internal.Btree.Algebra
{
	/// <exclude></exclude>
	public class BTreeRangeSingleIntersect : Db4objects.Db4o.Internal.Btree.Algebra.BTreeRangeSingleOperation
	{
		public BTreeRangeSingleIntersect(BTreeRangeSingle single) : base(single)
		{
		}

		protected override IBTreeRange Execute(BTreeRangeSingle single)
		{
			return BTreeAlgebra.Intersect(_single, single);
		}

		protected override IBTreeRange Execute(BTreeRangeUnion union)
		{
			return BTreeAlgebra.Intersect(union, _single);
		}
	}
}
