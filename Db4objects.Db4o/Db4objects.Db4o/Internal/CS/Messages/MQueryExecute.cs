namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MQueryExecute : Db4objects.Db4o.Internal.CS.Messages.MsgQuery
	{
		private Db4objects.Db4o.Config.QueryEvaluationMode _evaluationMode;

		public override bool ProcessAtServer(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
			 serverThread)
		{
			Unmarshall(_payLoad._offset);
			WriteQueryResult(Execute(), serverThread, _evaluationMode);
			return true;
		}

		private Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult Execute()
		{
			lock (StreamLock())
			{
				Db4objects.Db4o.Internal.Query.Processor.QQuery query = (Db4objects.Db4o.Internal.Query.Processor.QQuery
					)Stream().Unmarshall(_payLoad);
				query.Unmarshall(Transaction());
				_evaluationMode = query.EvaluationMode();
				return ExecuteFully(query);
			}
		}

		private Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult ExecuteFully(Db4objects.Db4o.Internal.Query.Processor.QQuery
			 query)
		{
			try
			{
				Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult qr = NewQueryResult(query
					.EvaluationMode());
				qr.LoadFromQuery(query);
				return qr;
			}
			catch
			{
				return NewQueryResult(query.EvaluationMode());
			}
		}
	}
}
