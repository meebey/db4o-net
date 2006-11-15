namespace Db4objects.Db4o.CS.Messages
{
	public sealed class MPrefetchIDs : Db4objects.Db4o.CS.Messages.MsgD
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.CS.YapServerThread serverThread
			)
		{
			int prefetchIDCount = ReadInt();
			Db4objects.Db4o.CS.Messages.MsgD reply = Db4objects.Db4o.CS.Messages.Msg.ID_LIST.
				GetWriterForLength(Transaction(), Db4objects.Db4o.YapConst.INT_LENGTH * prefetchIDCount
				);
			lock (StreamLock())
			{
				for (int i = 0; i < prefetchIDCount; i++)
				{
					reply.WriteInt(((Db4objects.Db4o.YapFile)Stream()).PrefetchID());
				}
			}
			serverThread.Write(reply);
			return true;
		}
	}
}
