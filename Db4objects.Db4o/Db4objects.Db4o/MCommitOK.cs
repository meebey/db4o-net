namespace Db4objects.Db4o
{
	internal sealed class MCommitOK : Db4objects.Db4o.Msg
	{
		internal sealed override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 @in)
		{
			GetTransaction().Commit();
			Db4objects.Db4o.Msg.OK.Write(GetStream(), @in);
			return true;
		}
	}
}
