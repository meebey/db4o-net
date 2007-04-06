using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MTaIsDeleted : MsgD, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			lock (StreamLock())
			{
				bool isDeleted = Transaction().IsDeleted(ReadInt());
				int ret = isDeleted ? 1 : 0;
				Write(Msg.TA_IS_DELETED.GetWriterForInt(Transaction(), ret));
			}
			return true;
		}
	}
}
