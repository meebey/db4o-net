namespace Db4objects.Db4o.CS.Messages
{
	public class MProcessDeletes : Db4objects.Db4o.CS.Messages.Msg
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.CS.YapServerThread serverThread
			)
		{
			lock (StreamLock())
			{
				Transaction().ProcessDeletes();
				return true;
			}
		}
	}
}
