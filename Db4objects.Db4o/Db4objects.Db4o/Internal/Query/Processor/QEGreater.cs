/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
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
			IPreparedComparison preparedComparison = constraint.PrepareComparison(candidate);
			if (preparedComparison is PreparedArrayContainsComparison)
			{
				return ((PreparedArrayContainsComparison)preparedComparison).IsSmallerThan(obj);
			}
			return preparedComparison.CompareTo(obj) < 0;
		}

		public override void IndexBitMap(bool[] bits)
		{
			bits[QE.Greater] = true;
		}
	}
}
