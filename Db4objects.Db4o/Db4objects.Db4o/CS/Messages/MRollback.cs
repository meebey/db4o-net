namespace Db4objects.Db4o.CS.Messages
{
	public sealed class MRollback : Db4objects.Db4o.CS.Messages.Msg
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.CS.YapServerThread serverThread
			)
		{
			Transaction().Rollback();
			return true;
		}
	}
}
