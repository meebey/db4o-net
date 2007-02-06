namespace Db4objects.Db4o.Internal.CS.Messages
{
	public class MProcessDeletes : Db4objects.Db4o.Internal.CS.Messages.Msg
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
			 serverThread)
		{
			lock (StreamLock())
			{
				Transaction().ProcessDeletes();
				return true;
			}
		}
	}
}
