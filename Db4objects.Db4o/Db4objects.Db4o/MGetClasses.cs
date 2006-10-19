namespace Db4objects.Db4o
{
	internal sealed class MGetClasses : Db4objects.Db4o.MsgD
	{
		internal sealed override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 sock)
		{
			Db4objects.Db4o.YapStream stream = GetStream();
			lock (stream.i_lock)
			{
				try
				{
					stream.ClassCollection().Write(GetTransaction());
				}
				catch (System.Exception e)
				{
				}
			}
			Db4objects.Db4o.MsgD message = Db4objects.Db4o.Msg.GET_CLASSES.GetWriterForLength
				(GetTransaction(), Db4objects.Db4o.YapConst.INT_LENGTH + 1);
			Db4objects.Db4o.YapReader writer = message.GetPayLoad();
			writer.WriteInt(stream.ClassCollection().GetID());
			writer.Append(stream.StringIO().EncodingByte());
			message.Write(stream, sock);
			return true;
		}
	}
}
