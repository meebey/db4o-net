namespace Db4objects.Db4o.CS.Messages
{
	internal sealed class MCommit : Db4objects.Db4o.CS.Messages.Msg
	{
		public sealed override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 @in)
		{
			GetTransaction().Commit();
			return true;
		}
	}
}
