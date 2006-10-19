namespace Db4objects.Db4o
{
	internal sealed class MGetAll : Db4objects.Db4o.Msg
	{
		internal sealed override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 sock)
		{
			Db4objects.Db4o.YapStream stream = GetStream();
			this.WriteQueryResult(GetTransaction(), GetAll(stream), sock);
			return true;
		}

		private Db4objects.Db4o.Inside.Query.IQueryResult GetAll(Db4objects.Db4o.YapStream
			 stream)
		{
			Db4objects.Db4o.Inside.Query.IQueryResult qr;
			lock (stream.i_lock)
			{
				try
				{
					qr = stream.GetAll(GetTransaction());
				}
				catch (System.Exception e)
				{
					qr = new Db4objects.Db4o.QueryResultImpl(GetTransaction());
				}
			}
			return qr;
		}
	}
}
