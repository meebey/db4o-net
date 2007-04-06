using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MGetThreadID : Msg, IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			RespondInt(ServerMessageDispatcher().DispatcherID());
			return true;
		}
	}
}
