namespace Db4objects.Db4o.Internal.Query.Processor
{
	/// <exclude></exclude>
	internal class QPending : Db4objects.Db4o.Foundation.Tree
	{
		internal readonly Db4objects.Db4o.Internal.Query.Processor.QConJoin _join;

		internal Db4objects.Db4o.Internal.Query.Processor.QCon _constraint;

		internal int _result;

		internal const int FALSE = -4;

		internal const int BOTH = 1;

		internal const int TRUE = 2;

		internal QPending(Db4objects.Db4o.Internal.Query.Processor.QConJoin a_join, Db4objects.Db4o.Internal.Query.Processor.QCon
			 a_constraint, bool a_firstResult)
		{
			_join = a_join;
			_constraint = a_constraint;
			_result = a_firstResult ? TRUE : FALSE;
		}

		public override int Compare(Db4objects.Db4o.Foundation.Tree a_to)
		{
			return _constraint.i_id - ((Db4objects.Db4o.Internal.Query.Processor.QPending)a_to
				)._constraint.i_id;
		}

		internal virtual void ChangeConstraint()
		{
			_constraint = _join.GetOtherConstraint(_constraint);
		}

		public override object ShallowClone()
		{
			Db4objects.Db4o.Internal.Query.Processor.QPending pending = new Db4objects.Db4o.Internal.Query.Processor.QPending
				(_join, _constraint, false);
			pending._result = _result;
			base.ShallowCloneInternal(pending);
			return pending;
		}

		public override object Key()
		{
			throw new System.NotImplementedException();
		}
	}
}
