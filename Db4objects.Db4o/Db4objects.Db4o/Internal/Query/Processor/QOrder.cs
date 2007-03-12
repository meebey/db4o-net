namespace Db4objects.Db4o.Internal.Query.Processor
{
	/// <exclude></exclude>
	internal class QOrder : Db4objects.Db4o.Foundation.Tree
	{
		internal static int equalityIDGenerator = 1;

		internal readonly Db4objects.Db4o.Internal.Query.Processor.QConObject _constraint;

		internal readonly Db4objects.Db4o.Internal.Query.Processor.QCandidate _candidate;

		private int _equalityID;

		internal QOrder(Db4objects.Db4o.Internal.Query.Processor.QConObject a_constraint, 
			Db4objects.Db4o.Internal.Query.Processor.QCandidate a_candidate)
		{
			_constraint = a_constraint;
			_candidate = a_candidate;
		}

		public virtual bool IsEqual(Db4objects.Db4o.Internal.Query.Processor.QOrder other
			)
		{
			if (other == null)
			{
				return false;
			}
			return _equalityID != 0 && _equalityID == other._equalityID;
		}

		public override int Compare(Db4objects.Db4o.Foundation.Tree a_to)
		{
			int res = InternalCompare();
			if (res != 0)
			{
				return res;
			}
			Db4objects.Db4o.Internal.Query.Processor.QOrder other = (Db4objects.Db4o.Internal.Query.Processor.QOrder
				)a_to;
			int equalityID = _equalityID;
			if (equalityID == 0)
			{
				if (other._equalityID != 0)
				{
					equalityID = other._equalityID;
				}
			}
			if (equalityID == 0)
			{
				equalityID = GenerateEqualityID();
			}
			_equalityID = equalityID;
			other._equalityID = equalityID;
			return res;
		}

		private int InternalCompare()
		{
			if (_constraint.i_comparator.IsSmaller(_candidate.Value()))
			{
				return -_constraint.Ordering();
			}
			if (_constraint.i_comparator.IsEqual(_candidate.Value()))
			{
				return 0;
			}
			return _constraint.Ordering();
		}

		public override object ShallowClone()
		{
			Db4objects.Db4o.Internal.Query.Processor.QOrder order = new Db4objects.Db4o.Internal.Query.Processor.QOrder
				(_constraint, _candidate);
			base.ShallowCloneInternal(order);
			return order;
		}

		public override object Key()
		{
			throw new System.NotImplementedException();
		}

		private static int GenerateEqualityID()
		{
			equalityIDGenerator++;
			if (equalityIDGenerator < 1)
			{
				equalityIDGenerator = 1;
			}
			return equalityIDGenerator;
		}
	}
}
