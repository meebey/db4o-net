namespace Db4objects.Db4o
{
	internal sealed class MTaIsDeleted : Db4objects.Db4o.MsgD
	{
		internal sealed override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 sock)
		{
			Db4objects.Db4o.YapStream stream = GetStream();
			lock (stream.i_lock)
			{
				bool isDeleted = GetTransaction().IsDeleted(this.ReadInt());
				int ret = isDeleted ? 1 : 0;
				Db4objects.Db4o.Msg.TA_IS_DELETED.GetWriterForInt(GetTransaction(), ret).Write(stream
					, sock);
			}
			return true;
		}
	}
}
