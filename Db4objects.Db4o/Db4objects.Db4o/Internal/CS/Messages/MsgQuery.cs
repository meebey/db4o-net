/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Config;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Messages;
using Db4objects.Db4o.Internal.Query.Result;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public abstract class MsgQuery : MsgObject
	{
		private const int IdAndSize = 2;

		private static int nextID;

		protected void WriteQueryResult(AbstractQueryResult queryResult, QueryEvaluationMode
			 evaluationMode)
		{
			int queryResultId = 0;
			int maxCount = 0;
			if (evaluationMode == QueryEvaluationMode.Immediate)
			{
				maxCount = queryResult.Size();
			}
			else
			{
				queryResultId = GenerateID();
				maxCount = Config().PrefetchObjectCount();
			}
			MsgD message = QueryResult.GetWriterForLength(Transaction(), BufferLength(maxCount
				));
			StatefulBuffer writer = message.PayLoad();
			writer.WriteInt(queryResultId);
			IIntIterator4 idIterator = queryResult.IterateIDs();
			writer.WriteIDs(idIterator, maxCount);
			if (queryResultId > 0)
			{
				IServerMessageDispatcher serverThread = ServerMessageDispatcher();
				serverThread.MapQueryResultToID(new LazyClientObjectSetStub(queryResult, idIterator
					), queryResultId);
			}
			Write(message);
		}

		private int BufferLength(int maxCount)
		{
			return Const4.IntLength * (maxCount + IdAndSize);
		}

		private static int GenerateID()
		{
			lock (typeof(MsgQuery))
			{
				nextID++;
				if (nextID < 0)
				{
					nextID = 1;
				}
				return nextID;
			}
		}

		protected virtual AbstractQueryResult NewQueryResult(QueryEvaluationMode mode)
		{
			return Stream().NewQueryResult(Transaction(), mode);
		}
	}
}
