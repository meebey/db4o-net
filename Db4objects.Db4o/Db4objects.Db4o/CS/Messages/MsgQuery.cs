namespace Db4objects.Db4o.CS.Messages
{
	public abstract class MsgQuery : Db4objects.Db4o.CS.Messages.MsgObject
	{
		private const int ID_AND_SIZE = 2;

		private static int nextID;

		protected void WriteQueryResult(Db4objects.Db4o.Inside.Query.AbstractQueryResult 
			queryResult, Db4objects.Db4o.CS.YapServerThread serverThread, Db4objects.Db4o.Config.QueryEvaluationMode
			 evaluationMode)
		{
			int queryResultId = 0;
			int maxCount = 0;
			if (evaluationMode == Db4objects.Db4o.Config.QueryEvaluationMode.IMMEDIATE)
			{
				queryResultId = GenerateID();
				maxCount = Config().PrefetchObjectCount();
			}
			else
			{
				maxCount = queryResult.Size();
			}
			Db4objects.Db4o.CS.Messages.MsgD message = QUERY_RESULT.GetWriterForLength(Transaction
				(), BufferLength(maxCount));
			Db4objects.Db4o.YapWriter writer = message.PayLoad();
			writer.WriteInt(queryResultId);
			Db4objects.Db4o.Foundation.IIntIterator4 idIterator = queryResult.IterateIDs();
			writer.WriteIDs(idIterator, maxCount);
			if (queryResultId > 0)
			{
				serverThread.MapQueryResultToID(new Db4objects.Db4o.CS.LazyClientObjectSetStub(queryResult
					, idIterator), queryResultId);
			}
			serverThread.Write(message);
		}

		private int BufferLength(int maxCount)
		{
			return Db4objects.Db4o.YapConst.INT_LENGTH * (maxCount + ID_AND_SIZE);
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

		protected virtual Db4objects.Db4o.Inside.Query.AbstractQueryResult NewQueryResult
			(Db4objects.Db4o.Config.QueryEvaluationMode mode)
		{
			return Stream().NewQueryResult(Transaction(), mode);
		}
	}
}
