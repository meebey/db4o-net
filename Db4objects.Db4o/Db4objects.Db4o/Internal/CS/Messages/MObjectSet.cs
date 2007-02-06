namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public abstract class MObjectSet : Db4objects.Db4o.Internal.CS.Messages.MsgD
	{
		protected virtual Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult QueryResult
			(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher serverThread, int queryResultID
			)
		{
			return Stub(serverThread, queryResultID).QueryResult();
		}

		protected virtual Db4objects.Db4o.Internal.CS.LazyClientObjectSetStub Stub(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
			 serverThread, int queryResultID)
		{
			return serverThread.QueryResultForID(queryResultID);
		}
	}
}
