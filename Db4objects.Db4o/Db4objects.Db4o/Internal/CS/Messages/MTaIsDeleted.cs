namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MTaIsDeleted : Db4objects.Db4o.Internal.CS.Messages.MsgD, Db4objects.Db4o.Internal.CS.Messages.IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			lock (StreamLock())
			{
				bool isDeleted = Transaction().IsDeleted(ReadInt());
				int ret = isDeleted ? 1 : 0;
				Write(Db4objects.Db4o.Internal.CS.Messages.Msg.TA_IS_DELETED.GetWriterForInt(Transaction
					(), ret));
			}
			return true;
		}
	}
}
