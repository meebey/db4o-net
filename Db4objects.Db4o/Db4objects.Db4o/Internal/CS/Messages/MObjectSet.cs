namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public abstract class MObjectSet : Db4objects.Db4o.Internal.CS.Messages.MsgD
	{
		protected virtual Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult QueryResult
			(int queryResultID)
		{
			return Stub(queryResultID).QueryResult();
		}

		protected virtual Db4objects.Db4o.Internal.CS.LazyClientObjectSetStub Stub(int queryResultID
			)
		{
			Db4objects.Db4o.Internal.CS.IServerMessageDispatcher serverThread = ServerMessageDispatcher
				();
			return serverThread.QueryResultForID(queryResultID);
		}
	}
}
