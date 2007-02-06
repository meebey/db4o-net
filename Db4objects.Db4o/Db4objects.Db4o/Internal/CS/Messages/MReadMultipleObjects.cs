namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MReadMultipleObjects : Db4objects.Db4o.Internal.CS.Messages.MsgD
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
			 serverThread)
		{
			int size = ReadInt();
			Db4objects.Db4o.Internal.CS.Messages.MsgD[] ret = new Db4objects.Db4o.Internal.CS.Messages.MsgD
				[size];
			int length = (1 + size) * Db4objects.Db4o.Internal.Const4.INT_LENGTH;
			lock (StreamLock())
			{
				for (int i = 0; i < size; i++)
				{
					int id = this._payLoad.ReadInt();
					try
					{
						Db4objects.Db4o.Internal.StatefulBuffer bytes = Stream().ReadWriterByID(Transaction
							(), id);
						if (bytes != null)
						{
							ret[i] = Db4objects.Db4o.Internal.CS.Messages.Msg.OBJECT_TO_CLIENT.GetWriter(bytes
								);
							length += ret[i]._payLoad.GetLength();
						}
					}
					catch (System.Exception e)
					{
					}
				}
			}
			Db4objects.Db4o.Internal.CS.Messages.MsgD multibytes = Db4objects.Db4o.Internal.CS.Messages.Msg
				.READ_MULTIPLE_OBJECTS.GetWriterForLength(Transaction(), length);
			multibytes.WriteInt(size);
			for (int i = 0; i < size; i++)
			{
				if (ret[i] == null)
				{
					multibytes.WriteInt(0);
				}
				else
				{
					multibytes.WriteInt(ret[i]._payLoad.GetLength());
					multibytes._payLoad.Append(ret[i]._payLoad._buffer);
				}
			}
			serverThread.Write(multibytes);
			return true;
		}
	}
}
