namespace Db4objects.Db4o.CS.Messages
{
	public sealed class MDelete : Db4objects.Db4o.CS.Messages.MsgD
	{
		public sealed override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 sock)
		{
			Db4objects.Db4o.YapReader bytes = this.GetByteLoad();
			Db4objects.Db4o.YapStream stream = GetStream();
			lock (stream.i_lock)
			{
				object obj = stream.GetByID1(GetTransaction(), bytes.ReadInt());
				bool userCall = bytes.ReadInt() == 1;
				if (obj != null)
				{
					try
					{
						stream.Delete1(GetTransaction(), obj, userCall);
					}
					catch (System.Exception e)
					{
					}
				}
			}
			return true;
		}
	}
}
