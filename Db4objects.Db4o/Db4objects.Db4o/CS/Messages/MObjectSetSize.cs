namespace Db4objects.Db4o.CS.Messages
{
	public class MObjectSetSize : Db4objects.Db4o.CS.Messages.MObjectSet
	{
		public override bool ProcessAtServer(Db4objects.Db4o.CS.YapServerThread serverThread
			)
		{
			Db4objects.Db4o.Inside.Query.AbstractQueryResult queryResult = QueryResult(serverThread
				, ReadInt());
			serverThread.Write(Db4objects.Db4o.CS.Messages.Msg.OBJECTSET_SIZE.GetWriterForInt
				(Transaction(), queryResult.Size()));
			return true;
		}
	}
}
