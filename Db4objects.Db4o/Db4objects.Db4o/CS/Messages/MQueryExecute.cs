namespace Db4objects.Db4o.CS.Messages
{
	public sealed class MQueryExecute : Db4objects.Db4o.CS.Messages.MsgObject
	{
		public override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 sock)
		{
			Unmarshall();
			WriteQueryResult(Execute(), sock);
			return true;
		}

		private Db4objects.Db4o.Inside.Query.IQueryResult Execute()
		{
			lock (StreamLock())
			{
				Db4objects.Db4o.Transaction trans = GetTransaction();
				Db4objects.Db4o.YapStream stream = GetStream();
				Db4objects.Db4o.QQuery query = (Db4objects.Db4o.QQuery)stream.Unmarshall(_payLoad
					);
				query.Unmarshall(trans);
				return ExecuteFully(trans, stream, query);
			}
		}

		private Db4objects.Db4o.Inside.Query.IQueryResult ExecuteFully(Db4objects.Db4o.Transaction
			 trans, Db4objects.Db4o.YapStream stream, Db4objects.Db4o.QQuery query)
		{
			try
			{
				Db4objects.Db4o.Inside.Query.IQueryResult qr = stream.NewQueryResult(trans);
				qr.LoadFromQuery(query);
				return qr;
			}
			catch
			{
				return stream.NewQueryResult(trans);
			}
		}
	}
}
