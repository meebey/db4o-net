namespace Db4objects.Db4o
{
	internal class MObjectByUuid : Db4objects.Db4o.MsgD
	{
		internal sealed override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 sock)
		{
			long uuid = ReadLong();
			byte[] signature = ReadBytes();
			int id = 0;
			Db4objects.Db4o.YapStream stream = GetStream();
			Db4objects.Db4o.Transaction trans = GetTransaction();
			lock (stream.i_lock)
			{
				try
				{
					object[] arr = trans.ObjectAndYapObjectBySignature(uuid, signature);
					if (arr[1] != null)
					{
						Db4objects.Db4o.YapObject yo = (Db4objects.Db4o.YapObject)arr[1];
						id = yo.GetID();
					}
				}
				catch (System.Exception e)
				{
				}
			}
			Db4objects.Db4o.Msg.OBJECT_BY_UUID.GetWriterForInt(trans, id).Write(stream, sock);
			return true;
		}
	}
}
