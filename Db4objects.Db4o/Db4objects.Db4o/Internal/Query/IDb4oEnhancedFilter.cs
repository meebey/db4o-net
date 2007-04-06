using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Internal.Query
{
	public interface IDb4oEnhancedFilter
	{
		void OptimizeQuery(IQuery query);
	}
}
