namespace Db4objects.Db4o
{
	internal sealed class MGetInternalIDs : Db4objects.Db4o.MsgD
	{
		internal sealed override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 sock)
		{
			Db4objects.Db4o.YapReader bytes = this.GetByteLoad();
			long[] ids;
			Db4objects.Db4o.YapStream stream = GetStream();
			lock (stream.i_lock)
			{
				try
				{
					ids = stream.GetYapClass(bytes.ReadInt()).GetIDs(GetTransaction());
				}
				catch (System.Exception e)
				{
					ids = new long[0];
				}
			}
			int size = ids.Length;
			Db4objects.Db4o.MsgD message = Db4objects.Db4o.Msg.ID_LIST.GetWriterForLength(GetTransaction
				(), Db4objects.Db4o.YapConst.ID_LENGTH * (size + 1));
			Db4objects.Db4o.YapReader writer = message.GetPayLoad();
			writer.WriteInt(size);
			for (int i = 0; i < size; i++)
			{
				writer.WriteInt((int)ids[i]);
			}
			message.Write(stream, sock);
			return true;
		}
	}
}
