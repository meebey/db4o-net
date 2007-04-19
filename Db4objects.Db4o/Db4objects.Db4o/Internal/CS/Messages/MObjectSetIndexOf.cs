using Db4objects.Db4o.Internal.CS.Messages;
using Db4objects.Db4o.Internal.Query.Result;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MObjectSetIndexOf : MObjectSet, IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			AbstractQueryResult queryResult = QueryResult(ReadInt());
			int id = queryResult.IndexOf(ReadInt());
			Write(Msg.OBJECTSET_INDEXOF.GetWriterForInt(Transaction(), id));
			return true;
		}
	}
}
