namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MRollback : Db4objects.Db4o.Internal.CS.Messages.Msg, Db4objects.Db4o.Internal.CS.Messages.IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			Transaction().Rollback();
			return true;
		}
	}
}
