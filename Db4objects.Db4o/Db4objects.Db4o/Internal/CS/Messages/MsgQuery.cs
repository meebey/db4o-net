namespace Db4objects.Db4o.Internal.CS.Messages
{
	public abstract class MsgQuery : Db4objects.Db4o.Internal.CS.Messages.MsgObject
	{
		private const int ID_AND_SIZE = 2;

		private static int nextID;

		protected void WriteQueryResult(Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult
			 queryResult, Db4objects.Db4o.Internal.CS.ServerMessageDispatcher serverThread, 
			Db4objects.Db4o.Config.QueryEvaluationMode evaluationMode)
		{
			int queryResultId = 0;
			int maxCount = 0;
			if (evaluationMode == Db4objects.Db4o.Config.QueryEvaluationMode.IMMEDIATE)
			{
				maxCount = queryResult.Size();
			}
			else
			{
				queryResultId = GenerateID();
				maxCount = Config().PrefetchObjectCount();
			}
			Db4objects.Db4o.Internal.CS.Messages.MsgD message = QUERY_RESULT.GetWriterForLength
				(Transaction(), BufferLength(maxCount));
			Db4objects.Db4o.Internal.StatefulBuffer writer = message.PayLoad();
			writer.WriteInt(queryResultId);
			Db4objects.Db4o.Foundation.IIntIterator4 idIterator = queryResult.IterateIDs();
			writer.WriteIDs(idIterator, maxCount);
			if (queryResultId > 0)
			{
				serverThread.MapQueryResultToID(new Db4objects.Db4o.Internal.CS.LazyClientObjectSetStub
					(queryResult, idIterator), queryResultId);
			}
			serverThread.Write(message);
		}

		private int BufferLength(int maxCount)
		{
			return Db4objects.Db4o.Internal.Const4.INT_LENGTH * (maxCount + ID_AND_SIZE);
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

		protected virtual Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult NewQueryResult
			(Db4objects.Db4o.Config.QueryEvaluationMode mode)
		{
			return Stream().NewQueryResult(Transaction(), mode);
		}
	}
}
