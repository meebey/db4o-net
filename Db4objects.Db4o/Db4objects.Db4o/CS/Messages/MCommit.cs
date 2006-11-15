namespace Db4objects.Db4o.CS.Messages
{
	internal sealed class MCommit : Db4objects.Db4o.CS.Messages.Msg
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.CS.YapServerThread serverThread
			)
		{
			Transaction().Commit();
			return true;
		}
	}
}
