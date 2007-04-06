using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MIdentity : Msg, IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			ObjectContainerBase stream = Stream();
			RespondInt((int)stream.GetID(stream.Identity()));
			return true;
		}
	}
}
