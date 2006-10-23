namespace Db4objects.Db4o
{
	internal sealed class MPrefetchIDs : Db4objects.Db4o.MsgD
	{
		internal sealed override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 sock)
		{
			Db4objects.Db4o.YapFile stream = (Db4objects.Db4o.YapFile)GetStream();
			int prefetchIDCount = ReadInt();
			Db4objects.Db4o.MsgD reply = Db4objects.Db4o.Msg.ID_LIST.GetWriterForLength(GetTransaction
				(), Db4objects.Db4o.YapConst.INT_LENGTH * prefetchIDCount);
			lock (stream.i_lock)
			{
				for (int i = 0; i < prefetchIDCount; i++)
				{
					reply.WriteInt(stream.PrefetchID());
				}
			}
			reply.Write(stream, sock);
			return true;
		}
	}
}