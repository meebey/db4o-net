namespace Db4objects.Db4o.Internal.Query.Processor
{
	/// <exclude></exclude>
	public class QEIdentity : Db4objects.Db4o.Internal.Query.Processor.QEEqual
	{
		public int i_objectID;

		public override bool Identity()
		{
			return true;
		}

		internal override bool Evaluate(Db4objects.Db4o.Internal.Query.Processor.QConObject
			 a_constraint, Db4objects.Db4o.Internal.Query.Processor.QCandidate a_candidate, 
			object a_value)
		{
			if (i_objectID == 0)
			{
				i_objectID = a_constraint.GetObjectID();
			}
			return a_candidate._key == i_objectID;
		}
	}
}
