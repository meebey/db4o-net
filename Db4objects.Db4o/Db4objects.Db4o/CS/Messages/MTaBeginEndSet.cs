namespace Db4objects.Db4o.CS.Messages
{
	public class MTaBeginEndSet : Db4objects.Db4o.CS.Messages.Msg
	{
		public sealed override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 @in)
		{
			lock (GetStream().i_lock)
			{
				GetTransaction().BeginEndSet();
				return true;
			}
		}
	}
}
