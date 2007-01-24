namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public class QEIdentity : Db4objects.Db4o.QEEqual
	{
		public int i_objectID;

		public override bool Identity()
		{
			return true;
		}

		internal override bool Evaluate(Db4objects.Db4o.QConObject a_constraint, Db4objects.Db4o.QCandidate
			 a_candidate, object a_value)
		{
			if (i_objectID == 0)
			{
				i_objectID = a_constraint.GetObjectID();
			}
			return a_candidate._key == i_objectID;
		}
	}
}
