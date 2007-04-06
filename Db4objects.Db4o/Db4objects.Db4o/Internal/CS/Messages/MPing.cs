using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MPing : Msg, IServerSideMessage, IClientSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			Write(Msg.OK);
			return true;
		}

		public virtual bool ProcessAtClient()
		{
			Write(Msg.PONG);
			return true;
		}
	}
}
