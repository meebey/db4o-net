namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MReadObject : Db4objects.Db4o.Internal.CS.Messages.MsgD, Db4objects.Db4o.Internal.CS.Messages.IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			Db4objects.Db4o.Internal.StatefulBuffer bytes = null;
			lock (StreamLock())
			{
				try
				{
					bytes = Stream().ReadWriterByID(Transaction(), _payLoad.ReadInt());
				}
				catch (System.Exception)
				{
				}
			}
			if (bytes == null)
			{
				bytes = new Db4objects.Db4o.Internal.StatefulBuffer(Transaction(), 0, 0);
			}
			Write(Db4objects.Db4o.Internal.CS.Messages.Msg.OBJECT_TO_CLIENT.GetWriter(bytes));
			return true;
		}
	}
}
