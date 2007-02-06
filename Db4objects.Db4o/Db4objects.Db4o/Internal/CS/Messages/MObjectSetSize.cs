namespace Db4objects.Db4o.Internal.CS.Messages
{
	public class MObjectSetSize : Db4objects.Db4o.Internal.CS.Messages.MObjectSet
	{
		public override bool ProcessAtServer(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
			 serverThread)
		{
			Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult queryResult = QueryResult
				(serverThread, ReadInt());
			serverThread.Write(Db4objects.Db4o.Internal.CS.Messages.Msg.OBJECTSET_SIZE.GetWriterForInt
				(Transaction(), queryResult.Size()));
			return true;
		}
	}
}
