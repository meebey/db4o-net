using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MPrefetchIDs : MsgD, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			int prefetchIDCount = ReadInt();
			MsgD reply = Msg.ID_LIST.GetWriterForLength(Transaction(), Const4.INT_LENGTH * prefetchIDCount
				);
			lock (StreamLock())
			{
				for (int i = 0; i < prefetchIDCount; i++)
				{
					reply.WriteInt(((LocalObjectContainer)Stream()).PrefetchID());
				}
			}
			Write(reply);
			return true;
		}
	}
}
