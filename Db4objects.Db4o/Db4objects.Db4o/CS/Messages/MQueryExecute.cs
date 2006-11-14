namespace Db4objects.Db4o.CS.Messages
{
	public sealed class MQueryExecute : Db4objects.Db4o.CS.Messages.MsgQuery
	{
		private Db4objects.Db4o.Config.QueryEvaluationMode _evaluationMode;

		public override bool ProcessAtServer(Db4objects.Db4o.CS.YapServerThread serverThread
			)
		{
			Unmarshall();
			WriteQueryResult(Execute(), serverThread, _evaluationMode);
			return true;
		}

		private Db4objects.Db4o.Inside.Query.AbstractQueryResult Execute()
		{
			lock (StreamLock())
			{
				Db4objects.Db4o.QQuery query = (Db4objects.Db4o.QQuery)Stream().Unmarshall(_payLoad
					);
				query.Unmarshall(Transaction());
				_evaluationMode = query.EvaluationMode();
				return ExecuteFully(query);
			}
		}

		private Db4objects.Db4o.Inside.Query.AbstractQueryResult ExecuteFully(Db4objects.Db4o.QQuery
			 query)
		{
			try
			{
				Db4objects.Db4o.Inside.Query.AbstractQueryResult qr = NewQueryResult(query.EvaluationMode
					());
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
