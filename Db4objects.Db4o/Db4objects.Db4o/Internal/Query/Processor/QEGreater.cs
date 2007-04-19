using Db4objects.Db4o.Internal.Query.Processor;

namespace Db4objects.Db4o.Internal.Query.Processor
{
	/// <exclude></exclude>
	public class QEGreater : QEAbstract
	{
		internal override bool Evaluate(QConObject a_constraint, QCandidate a_candidate, 
			object a_value)
		{
			if (a_value == null)
			{
				return false;
			}
			return a_constraint.GetComparator(a_candidate).IsGreater(a_value);
		}

		public override void IndexBitMap(bool[] bits)
		{
			bits[QE.GREATER] = true;
		}
	}
}
