namespace Db4objects.Db4o
{
	internal sealed class MReadMultipleObjects : Db4objects.Db4o.MsgD
	{
		internal sealed override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 sock)
		{
			int size = ReadInt();
			Db4objects.Db4o.MsgD[] ret = new Db4objects.Db4o.MsgD[size];
			int length = (1 + size) * Db4objects.Db4o.YapConst.INT_LENGTH;
			Db4objects.Db4o.YapStream stream = GetStream();
			Db4objects.Db4o.YapWriter bytes = null;
			lock (stream.i_lock)
			{
				for (int i = 0; i < size; i++)
				{
					int id = this._payLoad.ReadInt();
					try
					{
						bytes = stream.ReadWriterByID(GetTransaction(), id);
					}
					catch (System.Exception e)
					{
						bytes = null;
					}
					if (bytes != null)
					{
						ret[i] = Db4objects.Db4o.Msg.OBJECT_TO_CLIENT.GetWriter(bytes);
						length += ret[i]._payLoad.GetLength();
					}
				}
			}
			Db4objects.Db4o.MsgD multibytes = Db4objects.Db4o.Msg.READ_MULTIPLE_OBJECTS.GetWriterForLength
				(GetTransaction(), length);
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
			multibytes.Write(stream, sock);
			return true;
		}
	}
}
