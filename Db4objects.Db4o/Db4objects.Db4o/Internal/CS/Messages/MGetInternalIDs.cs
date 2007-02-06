namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MGetInternalIDs : Db4objects.Db4o.Internal.CS.Messages.MsgD
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
			 serverThread)
		{
			Db4objects.Db4o.Internal.Buffer bytes = this.GetByteLoad();
			long[] ids;
			lock (StreamLock())
			{
				try
				{
					ids = Stream().GetYapClass(bytes.ReadInt()).GetIDs(Transaction());
				}
				catch
				{
					ids = new long[0];
				}
			}
			int size = ids.Length;
			Db4objects.Db4o.Internal.CS.Messages.MsgD message = Db4objects.Db4o.Internal.CS.Messages.Msg
				.ID_LIST.GetWriterForLength(Transaction(), Db4objects.Db4o.Internal.Const4.ID_LENGTH
				 * (size + 1));
			Db4objects.Db4o.Internal.Buffer writer = message.PayLoad();
			writer.WriteInt(size);
			for (int i = 0; i < size; i++)
			{
				writer.WriteInt((int)ids[i]);
			}
			serverThread.Write(message);
			return true;
		}
	}
}
