namespace Db4objects.Db4o.CS.Messages
{
	public sealed class MCommitOK : Db4objects.Db4o.CS.Messages.Msg
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.CS.YapServerThread serverThread
			)
		{
			Transaction().Commit();
			serverThread.Write(Db4objects.Db4o.CS.Messages.Msg.OK);
			return true;
		}
	}
}
