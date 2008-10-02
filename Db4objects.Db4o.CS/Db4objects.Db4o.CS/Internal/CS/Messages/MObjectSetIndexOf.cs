/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.CS.Messages;
using Db4objects.Db4o.Internal.Query.Result;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MObjectSetIndexOf : MObjectSet, IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			AbstractQueryResult queryResult = QueryResult(ReadInt());
			lock (StreamLock())
			{
				int id = queryResult.IndexOf(ReadInt());
				Write(Msg.ObjectsetIndexof.GetWriterForInt(Transaction(), id));
			}
			return true;
		}
	}
}
