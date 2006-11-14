namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public class QConFalse : Db4objects.Db4o.QConPath
	{
		public QConFalse()
		{
		}

		internal QConFalse(Db4objects.Db4o.Transaction a_trans, Db4objects.Db4o.QCon a_parent
			, Db4objects.Db4o.QField a_field) : base(a_trans, a_parent, a_field)
		{
		}

		internal override void CreateCandidates(Db4objects.Db4o.Foundation.Collection4 a_candidateCollection
			)
		{
		}

		internal override bool Evaluate(Db4objects.Db4o.QCandidate a_candidate)
		{
			return false;
		}
	}
}
