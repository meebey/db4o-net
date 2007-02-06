namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MObjectSetGetId : Db4objects.Db4o.Internal.CS.Messages.MObjectSet
	{
		public override bool ProcessAtServer(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
			 serverThread)
		{
			Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult queryResult = QueryResult
				(serverThread, ReadInt());
			int id = queryResult.GetId(ReadInt());
			serverThread.Write(Db4objects.Db4o.Internal.CS.Messages.Msg.OBJECTSET_GET_ID.GetWriterForInt
				(Transaction(), id));
			return true;
		}
	}
}
