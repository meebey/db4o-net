namespace Db4objects.Db4o.Internal.Query.Processor
{
	/// <exclude></exclude>
	public class QConFalse : Db4objects.Db4o.Internal.Query.Processor.QConPath
	{
		public QConFalse()
		{
		}

		internal QConFalse(Db4objects.Db4o.Internal.Transaction a_trans, Db4objects.Db4o.Internal.Query.Processor.QCon
			 a_parent, Db4objects.Db4o.Internal.Query.Processor.QField a_field) : base(a_trans
			, a_parent, a_field)
		{
		}

		internal override void CreateCandidates(Db4objects.Db4o.Foundation.Collection4 a_candidateCollection
			)
		{
		}

		internal override bool Evaluate(Db4objects.Db4o.Internal.Query.Processor.QCandidate
			 a_candidate)
		{
			return false;
		}
	}
}
