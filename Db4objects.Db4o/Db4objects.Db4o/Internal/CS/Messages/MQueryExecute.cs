namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MQueryExecute : Db4objects.Db4o.Internal.CS.Messages.MsgQuery
		, Db4objects.Db4o.Internal.CS.Messages.IServerSideMessage
	{
		private Db4objects.Db4o.Config.QueryEvaluationMode _evaluationMode;

		public bool ProcessAtServer()
		{
			Unmarshall(_payLoad._offset);
			WriteQueryResult(Execute(), _evaluationMode);
			return true;
		}

		private Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult Execute()
		{
			lock (StreamLock())
			{
				Db4objects.Db4o.Internal.Query.Processor.QQuery query = (Db4objects.Db4o.Internal.Query.Processor.QQuery
					)ReadObjectFromPayLoad();
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
			catch (System.Exception)
			{
				return NewQueryResult(query.EvaluationMode());
			}
		}
	}
}
