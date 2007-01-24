namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public abstract class QEAbstract : Db4objects.Db4o.QE
	{
		internal override Db4objects.Db4o.QE Add(Db4objects.Db4o.QE evaluator)
		{
			Db4objects.Db4o.QE qe = new Db4objects.Db4o.QEMulti();
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
