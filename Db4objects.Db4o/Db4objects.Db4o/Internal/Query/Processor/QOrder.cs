namespace Db4objects.Db4o.Internal.Query.Processor
{
	/// <exclude></exclude>
	internal class QOrder : Db4objects.Db4o.Foundation.Tree
	{
		internal readonly Db4objects.Db4o.Internal.Query.Processor.QConObject _constraint;

		internal readonly Db4objects.Db4o.Internal.Query.Processor.QCandidate _candidate;

		internal QOrder(Db4objects.Db4o.Internal.Query.Processor.QConObject a_constraint, 
			Db4objects.Db4o.Internal.Query.Processor.QCandidate a_candidate)
		{
			_constraint = a_constraint;
			_candidate = a_candidate;
		}

		public override int Compare(Db4objects.Db4o.Foundation.Tree a_to)
		{
			if (_constraint.i_comparator.IsSmaller(_candidate.Value()))
			{
				return _constraint.Ordering();
			}
			if (_constraint.i_comparator.IsEqual(_candidate.Value()))
			{
				return 0;
			}
			return -_constraint.Ordering();
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
	}
}
