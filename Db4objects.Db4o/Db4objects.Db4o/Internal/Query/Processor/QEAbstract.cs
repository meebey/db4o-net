namespace Db4objects.Db4o.Internal.Query.Processor
{
	/// <exclude></exclude>
	public abstract class QEAbstract : Db4objects.Db4o.Internal.Query.Processor.QE
	{
		internal override Db4objects.Db4o.Internal.Query.Processor.QE Add(Db4objects.Db4o.Internal.Query.Processor.QE
			 evaluator)
		{
			Db4objects.Db4o.Internal.Query.Processor.QE qe = new Db4objects.Db4o.Internal.Query.Processor.QEMulti
				();
			qe.Add(this);
			qe.Add(evaluator);
			return qe;
		}

		internal override bool IsDefault()
		{
			return false;
		}
	}
}
