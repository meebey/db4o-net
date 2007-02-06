namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MDelete : Db4objects.Db4o.Internal.CS.Messages.MsgD
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
			 serverThread)
		{
			Db4objects.Db4o.Internal.Buffer bytes = this.GetByteLoad();
			Db4objects.Db4o.Internal.ObjectContainerBase stream = Stream();
			lock (StreamLock())
			{
				object obj = stream.GetByID1(Transaction(), bytes.ReadInt());
				bool userCall = bytes.ReadInt() == 1;
				if (obj != null)
				{
					try
					{
						stream.Delete1(Transaction(), obj, userCall);
					}
					catch
					{
					}
				}
			}
			return true;
		}
	}
}
