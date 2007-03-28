namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MUseTransaction : Db4objects.Db4o.Internal.CS.Messages.MsgD, 
		Db4objects.Db4o.Internal.CS.Messages.IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			Db4objects.Db4o.Internal.CS.IServerMessageDispatcher serverThread = ServerMessageDispatcher
				();
			serverThread.UseTransaction(this);
			return true;
		}
	}
}
