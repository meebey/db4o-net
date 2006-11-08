namespace Db4objects.Db4o.CS.Messages
{
	public sealed class MQueryExecute : Db4objects.Db4o.CS.Messages.MsgQuery
	{
		private bool _lazy = false;

		public override bool ProcessAtServer(Db4objects.Db4o.CS.YapServerThread serverThread
			)
		{
			Unmarshall();
			WriteQueryResult(Execute(), serverThread, _lazy);
			return true;
		}

		private Db4objects.Db4o.Inside.Query.AbstractQueryResult Execute()
		{
			lock (StreamLock())
			{
				Db4objects.Db4o.QQuery query = (Db4objects.Db4o.QQuery)Stream().Unmarshall(_payLoad
					);
				query.Unmarshall(Transaction());
				_lazy = query.IsLazy();
				return ExecuteFully(query);
			}
		}

		private Db4objects.Db4o.Inside.Query.AbstractQueryResult ExecuteFully(Db4objects.Db4o.QQuery
			 query)
		{
			try
			{
				Db4objects.Db4o.Inside.Query.AbstractQueryResult qr = NewQueryResult(query.IsLazy
					());
				qr.LoadFromQuery(query);
				return qr;
			}
			catch
			{
				return NewQueryResult(false);
			}
		}
	}
}
