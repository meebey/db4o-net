namespace Db4objects.Db4o.CS.Messages
{
	public sealed class MReadObject : Db4objects.Db4o.CS.Messages.MsgD
	{
		public sealed override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 sock)
		{
			Db4objects.Db4o.YapWriter bytes = null;
			Db4objects.Db4o.YapStream stream = GetStream();
			lock (stream.i_lock)
			{
				try
				{
					bytes = stream.ReadWriterByID(this.GetTransaction(), this._payLoad.ReadInt());
				}
				catch (System.Exception e)
				{
				}
			}
			if (bytes == null)
			{
				bytes = new Db4objects.Db4o.YapWriter(this.GetTransaction(), 0, 0);
			}
			Db4objects.Db4o.CS.Messages.Msg.OBJECT_TO_CLIENT.GetWriter(bytes).Write(stream, sock
				);
			return true;
		}
	}
}
