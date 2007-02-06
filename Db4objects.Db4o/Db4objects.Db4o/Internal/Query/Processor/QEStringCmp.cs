namespace Db4objects.Db4o.Internal.Query.Processor
{
	/// <exclude></exclude>
	public abstract class QEStringCmp : Db4objects.Db4o.Internal.Query.Processor.QEAbstract
	{
		public bool caseSensitive;

		public QEStringCmp(bool caseSensitive_)
		{
			caseSensitive = caseSensitive_;
		}

		internal override bool Evaluate(Db4objects.Db4o.Internal.Query.Processor.QConObject
			 a_constraint, Db4objects.Db4o.Internal.Query.Processor.QCandidate a_candidate, 
			object a_value)
		{
			if (a_value != null)
			{
				if (a_value is Db4objects.Db4o.Internal.Buffer)
				{
					a_value = a_candidate._marshallerFamily._string.ReadFromOwnSlot(a_constraint.i_trans
						.Stream(), ((Db4objects.Db4o.Internal.Buffer)a_value));
				}
				string candidate = a_value.ToString();
				string constraint = a_constraint.i_object.ToString();
				if (!caseSensitive)
				{
					candidate = candidate.ToLower();
					constraint = constraint.ToLower();
				}
				return CompareStrings(candidate, constraint);
			}
			return a_constraint.i_object == null;
		}

		public override bool SupportsIndex()
		{
			return false;
		}

		protected abstract bool CompareStrings(string candidate, string constraint);
	}
}
