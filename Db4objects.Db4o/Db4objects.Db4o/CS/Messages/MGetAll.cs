namespace Db4objects.Db4o.CS.Messages
{
	public sealed class MGetAll : Db4objects.Db4o.CS.Messages.Msg
	{
		public sealed override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 sock)
		{
			WriteQueryResult(GetAll(), sock);
			return true;
		}

		private Db4objects.Db4o.Inside.Query.IQueryResult GetAll()
		{
			lock (StreamLock())
			{
				try
				{
					return GetStream().GetAll(GetTransaction());
				}
				catch (System.Exception e)
				{
				}
				return GetStream().NewQueryResult(GetTransaction());
			}
		}
	}
}
