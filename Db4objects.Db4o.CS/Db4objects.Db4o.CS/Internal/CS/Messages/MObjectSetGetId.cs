/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.CS.Messages;
using Db4objects.Db4o.Internal.Query.Result;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MObjectSetGetId : MObjectSet, IMessageWithResponse
	{
		public virtual bool ProcessAtServer()
		{
			AbstractQueryResult queryResult = QueryResult(ReadInt());
			int id = 0;
			lock (StreamLock())
			{
				id = queryResult.GetId(ReadInt());
			}
			Write(Msg.ObjectsetGetId.GetWriterForInt(Transaction(), id));
			return true;
		}
	}
}
