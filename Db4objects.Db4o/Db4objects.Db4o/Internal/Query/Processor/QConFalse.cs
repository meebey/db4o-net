using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Query.Processor;

namespace Db4objects.Db4o.Internal.Query.Processor
{
	/// <exclude></exclude>
	public class QConFalse : QConPath
	{
		public QConFalse()
		{
		}

		internal QConFalse(Transaction a_trans, QCon a_parent, QField a_field) : base(a_trans
			, a_parent, a_field)
		{
		}

		internal override void CreateCandidates(Collection4 a_candidateCollection)
		{
		}

		internal override bool Evaluate(QCandidate a_candidate)
		{
			return false;
		}
	}
}
