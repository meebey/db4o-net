namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MCommitOK : Db4objects.Db4o.Internal.CS.Messages.Msg
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
			 serverThread)
		{
			Transaction().Commit();
			serverThread.Write(Db4objects.Db4o.Internal.CS.Messages.Msg.OK);
			return true;
		}
	}
}
