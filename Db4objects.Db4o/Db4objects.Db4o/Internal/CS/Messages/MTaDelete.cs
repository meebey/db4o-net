namespace Db4objects.Db4o.Internal.CS.Messages
{
	public class MTaDelete : Db4objects.Db4o.Internal.CS.Messages.MsgD
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
			 serverThread)
		{
			int id = _payLoad.ReadInt();
			int cascade = _payLoad.ReadInt();
			Db4objects.Db4o.Internal.Transaction trans = Transaction();
			lock (StreamLock())
			{
				trans.Delete(null, id, cascade);
				return true;
			}
		}
	}
}
