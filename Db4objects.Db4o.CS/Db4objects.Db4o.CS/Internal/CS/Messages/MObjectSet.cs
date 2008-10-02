/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Messages;
using Db4objects.Db4o.Internal.Query.Result;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public abstract class MObjectSet : MsgD
	{
		protected virtual AbstractQueryResult QueryResult(int queryResultID)
		{
			return Stub(queryResultID).QueryResult();
		}

		protected virtual LazyClientObjectSetStub Stub(int queryResultID)
		{
			IServerMessageDispatcher serverThread = ServerMessageDispatcher();
			return serverThread.QueryResultForID(queryResultID);
		}
	}
}
