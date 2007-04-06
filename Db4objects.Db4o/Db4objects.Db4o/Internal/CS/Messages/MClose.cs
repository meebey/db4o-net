using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MClose : Msg, IServerSideMessage, IClientSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			Transaction().Commit();
			LogMsg(35, ServerMessageDispatcher().Name());
			ServerMessageDispatcher().Close();
			return true;
		}

		public virtual bool ProcessAtClient()
		{
			ClientMessageDispatcher().Close();
			return true;
		}
	}
}
