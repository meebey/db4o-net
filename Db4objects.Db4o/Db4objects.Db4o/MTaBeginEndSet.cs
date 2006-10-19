namespace Db4objects.Db4o
{
	internal class MTaBeginEndSet : Db4objects.Db4o.Msg
	{
		internal sealed override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
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
