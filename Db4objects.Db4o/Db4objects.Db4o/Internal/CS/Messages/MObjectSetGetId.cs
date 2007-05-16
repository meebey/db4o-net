/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.CS.Messages;
using Db4objects.Db4o.Internal.Query.Result;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MObjectSetGetId : MObjectSet, IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			AbstractQueryResult queryResult = QueryResult(ReadInt());
			int id = queryResult.GetId(ReadInt());
			Write(Msg.OBJECTSET_GET_ID.GetWriterForInt(Transaction(), id));
			return true;
		}
	}
}
