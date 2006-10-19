namespace Db4objects.Db4o
{
	internal sealed class MCommit : Db4objects.Db4o.Msg
	{
		internal sealed override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 @in)
		{
			GetTransaction().Commit();
			return true;
		}
	}
}
