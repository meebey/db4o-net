namespace Db4objects.Db4o.Internal.Query
{
	public interface IDb4oNQOptimizer
	{
		object Optimize(Db4objects.Db4o.Query.IQuery query, Db4objects.Db4o.Query.Predicate
			 filter);
	}
}
