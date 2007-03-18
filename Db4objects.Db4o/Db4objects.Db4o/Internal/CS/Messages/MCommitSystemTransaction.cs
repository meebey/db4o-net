namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MCommitSystemTransaction : Db4objects.Db4o.Internal.CS.Messages.Msg
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
			 serverThread)
		{
			Transaction().SystemTransaction().Commit();
			return true;
		}
	}
}
