using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MSwitchToMainFile : Msg, IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			IServerMessageDispatcher serverThread = ServerMessageDispatcher();
			serverThread.SwitchToMainFile();
			return true;
		}
	}
}
