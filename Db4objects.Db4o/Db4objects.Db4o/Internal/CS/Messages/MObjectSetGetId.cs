/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
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
			try
			{
				int id = queryResult.GetId(ReadInt());
				Write(Msg.ObjectsetGetId.GetWriterForInt(Transaction(), id));
			}
			catch (IndexOutOfRangeException e)
			{
				WriteException(e);
			}
			return true;
		}
	}
}
