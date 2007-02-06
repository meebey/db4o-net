namespace Db4objects.Db4o.Internal.CS.Messages
{
	internal sealed class MCommit : Db4objects.Db4o.Internal.CS.Messages.Msg
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
			 serverThread)
		{
			Transaction().Commit();
			return true;
		}
	}
}
