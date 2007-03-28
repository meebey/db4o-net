namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MSwitchToMainFile : Db4objects.Db4o.Internal.CS.Messages.Msg, Db4objects.Db4o.Internal.CS.Messages.IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			Db4objects.Db4o.Internal.CS.IServerMessageDispatcher serverThread = ServerMessageDispatcher
				();
			serverThread.SwitchToMainFile();
			return true;
		}
	}
}
