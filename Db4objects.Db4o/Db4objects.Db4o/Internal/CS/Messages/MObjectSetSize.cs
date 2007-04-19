using Db4objects.Db4o.Internal.CS.Messages;
using Db4objects.Db4o.Internal.Query.Result;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public class MObjectSetSize : MObjectSet, IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			AbstractQueryResult queryResult = QueryResult(ReadInt());
			Write(Msg.OBJECTSET_SIZE.GetWriterForInt(Transaction(), queryResult.Size()));
			return true;
		}
	}
}
