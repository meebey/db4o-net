/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Query.Processor;

namespace Db4objects.Db4o.Internal.Query.Processor
{
	/// <exclude></exclude>
	public class QEGreater : QEAbstract
	{
		internal override bool Evaluate(QConObject constraint, QCandidate candidate, object
			 obj)
		{
			if (obj == null)
			{
				return false;
			}
			IComparable4 comparator = constraint.GetComparator(candidate);
			if (comparator is ArrayHandler)
			{
				return ((ArrayHandler)comparator).IsGreater(obj);
			}
			return comparator.CompareTo(obj) > 0;
		}

		public override void IndexBitMap(bool[] bits)
		{
			bits[QE.GREATER] = true;
		}
	}
}
