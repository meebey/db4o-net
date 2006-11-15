namespace Db4objects.Db4o.CS.Messages
{
	public sealed class MReadObject : Db4objects.Db4o.CS.Messages.MsgD
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.CS.YapServerThread serverThread
			)
		{
			Db4objects.Db4o.YapWriter bytes = null;
			lock (StreamLock())
			{
				try
				{
					bytes = Stream().ReadWriterByID(Transaction(), _payLoad.ReadInt());
				}
				catch
				{
				}
			}
			if (bytes == null)
			{
				bytes = new Db4objects.Db4o.YapWriter(Transaction(), 0, 0);
			}
			serverThread.Write(Db4objects.Db4o.CS.Messages.Msg.OBJECT_TO_CLIENT.GetWriter(bytes
				));
			return true;
		}
	}
}
