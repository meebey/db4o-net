namespace Db4objects.Db4o.CS.Messages
{
	public sealed class MDelete : Db4objects.Db4o.CS.Messages.MsgD
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.CS.YapServerThread serverThread
			)
		{
			Db4objects.Db4o.YapReader bytes = this.GetByteLoad();
			Db4objects.Db4o.YapStream stream = Stream();
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
