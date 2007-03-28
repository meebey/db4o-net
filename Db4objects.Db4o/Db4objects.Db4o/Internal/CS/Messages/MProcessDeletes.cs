namespace Db4objects.Db4o.Internal.CS.Messages
{
	public class MProcessDeletes : Db4objects.Db4o.Internal.CS.Messages.Msg, Db4objects.Db4o.Internal.CS.Messages.IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			lock (StreamLock())
			{
				Transaction().ProcessDeletes();
				return true;
			}
		}
	}
}
