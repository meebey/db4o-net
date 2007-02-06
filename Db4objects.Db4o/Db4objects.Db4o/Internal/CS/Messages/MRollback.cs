namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MRollback : Db4objects.Db4o.Internal.CS.Messages.Msg
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
			 serverThread)
		{
			Transaction().Rollback();
			return true;
		}
	}
}
