namespace Db4objects.Db4o.CS.Messages
{
	public sealed class MSetSemaphore : Db4objects.Db4o.CS.Messages.MsgD
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.CS.YapServerThread serverThread
			)
		{
			int timeout = ReadInt();
			string name = ReadString();
			Db4objects.Db4o.YapFile stream = (Db4objects.Db4o.YapFile)Stream();
			bool res = stream.SetSemaphore(Transaction(), name, timeout);
			if (res)
			{
				serverThread.Write(Db4objects.Db4o.CS.Messages.Msg.SUCCESS);
			}
			else
			{
				serverThread.Write(Db4objects.Db4o.CS.Messages.Msg.FAILED);
			}
			return true;
		}
	}
}
