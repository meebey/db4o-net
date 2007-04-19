using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MWriteUpdateDeleteMembers : MsgD, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			lock (StreamLock())
			{
				Transaction().WriteUpdateDeleteMembers(ReadInt(), Stream().ClassMetadataForId(ReadInt
					()), ReadInt(), ReadInt());
			}
			return true;
		}
	}
}
