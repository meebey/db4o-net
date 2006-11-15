namespace Db4objects.Db4o.CS.Messages
{
	public sealed class MTaIsDeleted : Db4objects.Db4o.CS.Messages.MsgD
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.CS.YapServerThread serverThread
			)
		{
			lock (StreamLock())
			{
				bool isDeleted = Transaction().IsDeleted(ReadInt());
				int ret = isDeleted ? 1 : 0;
				serverThread.Write(Db4objects.Db4o.CS.Messages.Msg.TA_IS_DELETED.GetWriterForInt(
					Transaction(), ret));
			}
			return true;
		}
	}
}
