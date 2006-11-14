namespace Db4objects.Db4o.CS.Messages
{
	public sealed class MGetClasses : Db4objects.Db4o.CS.Messages.MsgD
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.CS.YapServerThread serverThread
			)
		{
			Db4objects.Db4o.YapStream stream = Stream();
			lock (StreamLock())
			{
				try
				{
					stream.ClassCollection().Write(Transaction());
				}
				catch
				{
				}
			}
			Db4objects.Db4o.CS.Messages.MsgD message = Db4objects.Db4o.CS.Messages.Msg.GET_CLASSES
				.GetWriterForLength(Transaction(), Db4objects.Db4o.YapConst.INT_LENGTH + 1);
			Db4objects.Db4o.YapReader writer = message.PayLoad();
			writer.WriteInt(stream.ClassCollection().GetID());
			writer.Append(stream.StringIO().EncodingByte());
			serverThread.Write(message);
			return true;
		}
	}
}
