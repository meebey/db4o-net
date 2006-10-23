namespace Db4objects.Db4o.Inside.Query
{
	public interface IDb4oQueryExecutionListener
	{
		void NotifyQueryExecuted(Db4objects.Db4o.Inside.Query.NQOptimizationInfo info);
	}
}
