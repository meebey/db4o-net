namespace Db4objects.Db4o
{
	internal sealed class MQueryExecute : Db4objects.Db4o.MsgObject
	{
		internal override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 sock)
		{
			Db4objects.Db4o.Transaction trans = GetTransaction();
			Db4objects.Db4o.YapStream stream = GetStream();
			Db4objects.Db4o.QueryResultImpl qr = new Db4objects.Db4o.QueryResultImpl(trans);
			this.Unmarshall();
			lock (stream.i_lock)
			{
				Db4objects.Db4o.QQuery query = (Db4objects.Db4o.QQuery)stream.Unmarshall(_payLoad
					);
				query.Unmarshall(GetTransaction());
				try
				{
					query.ExecuteLocal(qr);
				}
				catch (System.Exception e)
				{
					qr = new Db4objects.Db4o.QueryResultImpl(GetTransaction());
				}
			}
			WriteQueryResult(GetTransaction(), qr, sock);
			return true;
		}
	}
}
