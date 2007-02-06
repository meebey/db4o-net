namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MObjectSetIndexOf : Db4objects.Db4o.Internal.CS.Messages.MObjectSet
	{
		public override bool ProcessAtServer(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
			 serverThread)
		{
			Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult queryResult = QueryResult
				(serverThread, ReadInt());
			int id = queryResult.IndexOf(ReadInt());
			serverThread.Write(Db4objects.Db4o.Internal.CS.Messages.Msg.OBJECTSET_INDEXOF.GetWriterForInt
				(Transaction(), id));
			return true;
		}
	}
}
