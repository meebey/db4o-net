namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MReadObject : Db4objects.Db4o.Internal.CS.Messages.MsgD
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
			 serverThread)
		{
			Db4objects.Db4o.Internal.StatefulBuffer bytes = null;
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
				bytes = new Db4objects.Db4o.Internal.StatefulBuffer(Transaction(), 0, 0);
			}
			serverThread.Write(Db4objects.Db4o.Internal.CS.Messages.Msg.OBJECT_TO_CLIENT.GetWriter
				(bytes));
			return true;
		}
	}
}
