namespace Db4objects.Db4o.Internal.Query.Processor
{
	/// <exclude></exclude>
	public class QEMulti : Db4objects.Db4o.Internal.Query.Processor.QE
	{
		public Db4objects.Db4o.Foundation.Collection4 i_evaluators = new Db4objects.Db4o.Foundation.Collection4
			();

		internal override Db4objects.Db4o.Internal.Query.Processor.QE Add(Db4objects.Db4o.Internal.Query.Processor.QE
			 evaluator)
		{
			i_evaluators.Ensure(evaluator);
			return this;
		}

		public override bool Identity()
		{
			bool ret = false;
			System.Collections.IEnumerator i = i_evaluators.GetEnumerator();
			while (i.MoveNext())
			{
				if (((Db4objects.Db4o.Internal.Query.Processor.QE)i.Current).Identity())
				{
					ret = true;
				}
				else
				{
					return false;
				}
			}
			return ret;
		}

		internal override bool IsDefault()
		{
			return false;
		}

		internal override bool Evaluate(Db4objects.Db4o.Internal.Query.Processor.QConObject
			 a_constraint, Db4objects.Db4o.Internal.Query.Processor.QCandidate a_candidate, 
			object a_value)
		{
			System.Collections.IEnumerator i = i_evaluators.GetEnumerator();
			while (i.MoveNext())
			{
				if (((Db4objects.Db4o.Internal.Query.Processor.QE)i.Current).Evaluate(a_constraint
					, a_candidate, a_value))
				{
					return true;
				}
			}
			return false;
		}

		public override void IndexBitMap(bool[] bits)
		{
			System.Collections.IEnumerator i = i_evaluators.GetEnumerator();
			while (i.MoveNext())
			{
				((Db4objects.Db4o.Internal.Query.Processor.QE)i.Current).IndexBitMap(bits);
			}
		}

		public override bool SupportsIndex()
		{
			System.Collections.IEnumerator i = i_evaluators.GetEnumerator();
			while (i.MoveNext())
			{
				if (!((Db4objects.Db4o.Internal.Query.Processor.QE)i.Current).SupportsIndex())
				{
					return false;
				}
			}
			return true;
		}
	}
}
