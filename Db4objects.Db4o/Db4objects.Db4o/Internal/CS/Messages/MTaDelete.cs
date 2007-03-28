namespace Db4objects.Db4o.Internal.CS.Messages
{
	public class MTaDelete : Db4objects.Db4o.Internal.CS.Messages.MsgD, Db4objects.Db4o.Internal.CS.Messages.IServerSideMessage
	{
		public bool ProcessAtServer()
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
