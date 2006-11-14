namespace Db4objects.Db4o.CS.Messages
{
	/// <exclude></exclude>
	public abstract class MObjectSet : Db4objects.Db4o.CS.Messages.MsgD
	{
		protected virtual Db4objects.Db4o.Inside.Query.AbstractQueryResult QueryResult(Db4objects.Db4o.CS.YapServerThread
			 serverThread, int queryResultID)
		{
			return Stub(serverThread, queryResultID).QueryResult();
		}

		protected virtual Db4objects.Db4o.CS.LazyClientObjectSetStub Stub(Db4objects.Db4o.CS.YapServerThread
			 serverThread, int queryResultID)
		{
			return serverThread.QueryResultForID(queryResultID);
		}
	}
}
