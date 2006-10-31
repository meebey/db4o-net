namespace Db4objects.Db4o.CS.Messages
{
	public sealed class MGetClasses : Db4objects.Db4o.CS.Messages.MsgD
	{
		public sealed override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 sock)
		{
			Db4objects.Db4o.YapStream stream = GetStream();
			lock (stream.i_lock)
			{
				try
				{
					stream.ClassCollection().Write(GetTransaction());
				}
				catch
				{
				}
			}
			Db4objects.Db4o.CS.Messages.MsgD message = Db4objects.Db4o.CS.Messages.Msg.GET_CLASSES
				.GetWriterForLength(GetTransaction(), Db4objects.Db4o.YapConst.INT_LENGTH + 1);
			Db4objects.Db4o.YapReader writer = message.PayLoad();
			writer.WriteInt(stream.ClassCollection().GetID());
			writer.Append(stream.StringIO().EncodingByte());
			message.Write(stream, sock);
			return true;
		}
	}
}
