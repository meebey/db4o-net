namespace Db4objects.Db4o.CS.Messages
{
	public class MTaBeginEndSet : Db4objects.Db4o.CS.Messages.Msg
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.CS.YapServerThread serverThread
			)
		{
			lock (StreamLock())
			{
				Transaction().BeginEndSet();
				return true;
			}
		}
	}
}
