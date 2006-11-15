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
				object[] arr = Stream().GetObjectAndYapObjectByID(trans, id);
				trans.Delete((Db4objects.Db4o.YapObject)arr[1], cascade);
				return true;
			}
		}
	}
}
