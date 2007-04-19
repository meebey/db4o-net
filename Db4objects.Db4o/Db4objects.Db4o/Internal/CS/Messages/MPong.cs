using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MPong : Msg, IServerSideMessage, IClientSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			return true;
		}

		public virtual bool ProcessAtClient()
		{
			return true;
		}
	}
}
