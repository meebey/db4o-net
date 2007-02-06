namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MSetSemaphore : Db4objects.Db4o.Internal.CS.Messages.MsgD
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
			 serverThread)
		{
			int timeout = ReadInt();
			string name = ReadString();
			Db4objects.Db4o.Internal.LocalObjectContainer stream = (Db4objects.Db4o.Internal.LocalObjectContainer
				)Stream();
			bool res = stream.SetSemaphore(Transaction(), name, timeout);
			if (res)
			{
				serverThread.Write(Db4objects.Db4o.Internal.CS.Messages.Msg.SUCCESS);
			}
			else
			{
				serverThread.Write(Db4objects.Db4o.Internal.CS.Messages.Msg.FAILED);
			}
			return true;
		}
	}
}
