namespace Db4objects.Db4o.CS.Messages
{
	/// <exclude></exclude>
	public class MObjectSetGetId : Db4objects.Db4o.CS.Messages.MObjectSet
	{
		public override bool ProcessAtServer(Db4objects.Db4o.CS.YapServerThread serverThread
			)
		{
			Db4objects.Db4o.Inside.Query.AbstractQueryResult queryResult = QueryResult(serverThread
				, ReadInt());
			int id = queryResult.GetId(ReadInt());
			serverThread.Write(Db4objects.Db4o.CS.Messages.Msg.OBJECTSET_GET_ID.GetWriterForInt
				(Transaction(), id));
			return true;
		}
	}
}
