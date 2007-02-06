namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MPrefetchIDs : Db4objects.Db4o.Internal.CS.Messages.MsgD
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
			 serverThread)
		{
			int prefetchIDCount = ReadInt();
			Db4objects.Db4o.Internal.CS.Messages.MsgD reply = Db4objects.Db4o.Internal.CS.Messages.Msg
				.ID_LIST.GetWriterForLength(Transaction(), Db4objects.Db4o.Internal.Const4.INT_LENGTH
				 * prefetchIDCount);
			lock (StreamLock())
			{
				for (int i = 0; i < prefetchIDCount; i++)
				{
					reply.WriteInt(((Db4objects.Db4o.Internal.LocalObjectContainer)Stream()).PrefetchID
						());
				}
			}
			serverThread.Write(reply);
			return true;
		}
	}
}
