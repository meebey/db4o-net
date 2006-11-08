namespace Db4objects.Db4o.CS.Messages
{
	public sealed class MGetAll : Db4objects.Db4o.CS.Messages.MsgQuery
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.CS.YapServerThread serverThread
			)
		{
			bool lazy = ReadBoolean();
			WriteQueryResult(GetAll(lazy), serverThread, lazy);
			return true;
		}

		private Db4objects.Db4o.Inside.Query.AbstractQueryResult GetAll(bool lazy)
		{
			lock (StreamLock())
			{
				try
				{
					return File().GetAll(Transaction(), lazy);
				}
				catch (System.Exception e)
				{
				}
				return NewQueryResult(false);
			}
		}
	}
}
