namespace Db4objects.Db4o.Inside.Query
{
	/// <exclude></exclude>
	[System.Serializable]
	public class PredicateEvaluation : Db4objects.Db4o.Query.IEvaluation
	{
		public Db4objects.Db4o.Query.Predicate _predicate;

		public PredicateEvaluation()
		{
		}

		public PredicateEvaluation(Db4objects.Db4o.Query.Predicate predicate)
		{
			_predicate = predicate;
		}

		public virtual void Evaluate(Db4objects.Db4o.Query.ICandidate candidate)
		{
			candidate.Include(_predicate.AppliesTo(candidate.GetObject()));
		}
	}
}
