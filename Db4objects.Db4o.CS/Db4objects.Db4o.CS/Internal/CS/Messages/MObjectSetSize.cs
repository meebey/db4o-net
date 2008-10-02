/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.CS.Messages;
using Db4objects.Db4o.Internal.Query.Result;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public class MObjectSetSize : MObjectSet, IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			MsgD writer = null;
			lock (StreamLock())
			{
				AbstractQueryResult queryResult = QueryResult(ReadInt());
				writer = Msg.ObjectsetSize.GetWriterForInt(Transaction(), queryResult.Size());
			}
			Write(writer);
			return true;
		}
	}
}
