namespace Db4objects.Db4o.CS.Messages
{
	public class MTaDelete : Db4objects.Db4o.CS.Messages.MsgD
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.CS.YapServerThread serverThread
			)
		{
			int id = _payLoad.ReadInt();
			int cascade = _payLoad.ReadInt();
			Db4objects.Db4o.Transaction trans = Transaction();
			lock (StreamLock())
			{
				trans.Delete(null, id, cascade);
				return true;
			}
		}
	}
}
