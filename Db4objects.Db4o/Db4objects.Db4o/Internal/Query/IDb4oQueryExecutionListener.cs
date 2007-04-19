using Db4objects.Db4o.Internal.Query;

namespace Db4objects.Db4o.Internal.Query
{
	public interface IDb4oQueryExecutionListener
	{
		void NotifyQueryExecuted(NQOptimizationInfo info);
	}
}
