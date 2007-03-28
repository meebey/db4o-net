namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MSwitchToFile : Db4objects.Db4o.Internal.CS.Messages.MsgD, Db4objects.Db4o.Internal.CS.Messages.IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			Db4objects.Db4o.Internal.CS.IServerMessageDispatcher serverThread = ServerMessageDispatcher
				();
			serverThread.SwitchToFile(this);
			return true;
		}
	}
}
