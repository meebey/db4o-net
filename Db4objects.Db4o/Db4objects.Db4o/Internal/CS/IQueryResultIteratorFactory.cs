namespace Db4objects.Db4o.Internal.CS
{
	public interface IQueryResultIteratorFactory
	{
		System.Collections.IEnumerator NewInstance(Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult
			 result);
	}
}
