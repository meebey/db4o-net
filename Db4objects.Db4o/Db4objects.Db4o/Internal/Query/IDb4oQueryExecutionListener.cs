namespace Db4objects.Db4o.Internal.Query
{
	public interface IDb4oQueryExecutionListener
	{
		void NotifyQueryExecuted(Db4objects.Db4o.Internal.Query.NQOptimizationInfo info);
	}
}
