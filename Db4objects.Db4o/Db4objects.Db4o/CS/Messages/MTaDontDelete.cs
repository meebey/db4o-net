namespace Db4objects.Db4o.CS.Messages
{
	public class MTaDontDelete : Db4objects.Db4o.CS.Messages.MsgD
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.CS.YapServerThread serverThread
			)
		{
			int classID = _payLoad.ReadInt();
			int id = _payLoad.ReadInt();
			lock (StreamLock())
			{
				Transaction().DontDelete(classID, id);
				return true;
			}
		}
	}
}
