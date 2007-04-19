using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MUseTransaction : MsgD, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			IServerMessageDispatcher serverThread = ServerMessageDispatcher();
			serverThread.UseTransaction(this);
			return true;
		}
	}
}
