using System.Collections;
using Db4objects.Db4o.Internal.Query.Result;

namespace Db4objects.Db4o.Internal.CS
{
	public interface IQueryResultIteratorFactory
	{
		IEnumerator NewInstance(AbstractQueryResult result);
	}
}
