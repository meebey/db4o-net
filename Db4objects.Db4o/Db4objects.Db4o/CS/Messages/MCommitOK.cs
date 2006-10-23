namespace Db4objects.Db4o.CS.Messages
{
	public sealed class MCommitOK : Db4objects.Db4o.CS.Messages.Msg
	{
		public sealed override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 @in)
		{
			GetTransaction().Commit();
			Db4objects.Db4o.CS.Messages.Msg.OK.Write(GetStream(), @in);
			return true;
		}
	}
}
