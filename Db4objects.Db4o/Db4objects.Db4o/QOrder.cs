namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	internal class QOrder : Db4objects.Db4o.Foundation.Tree
	{
		internal readonly Db4objects.Db4o.QConObject _constraint;

		internal readonly Db4objects.Db4o.QCandidate _candidate;

		internal QOrder(Db4objects.Db4o.QConObject a_constraint, Db4objects.Db4o.QCandidate
			 a_candidate)
		{
			_constraint = a_constraint;
			_candidate = a_candidate;
		}

		public override int Compare(Db4objects.Db4o.Foundation.Tree a_to)
		{
			if (_constraint.i_comparator.IsSmaller(_candidate.Value()))
			{
				return _constraint.i_orderID;
			}
			if (_constraint.i_comparator.IsEqual(_candidate.Value()))
			{
				return 0;
			}
			return -_constraint.i_orderID;
		}

		public override object ShallowClone()
		{
			Db4objects.Db4o.QOrder order = new Db4objects.Db4o.QOrder(_constraint, _candidate
				);
			base.ShallowCloneInternal(order);
			return order;
		}
	}
}
