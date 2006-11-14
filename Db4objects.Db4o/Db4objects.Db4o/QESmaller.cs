namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public class QESmaller : Db4objects.Db4o.QEAbstract
	{
		internal override bool Evaluate(Db4objects.Db4o.QConObject a_constraint, Db4objects.Db4o.QCandidate
			 a_candidate, object a_value)
		{
			if (a_value == null)
			{
				return false;
			}
			return a_constraint.GetComparator(a_candidate).IsSmaller(a_value);
		}

		public override void IndexBitMap(bool[] bits)
		{
			bits[Db4objects.Db4o.QE.SMALLER] = true;
		}
	}
}
