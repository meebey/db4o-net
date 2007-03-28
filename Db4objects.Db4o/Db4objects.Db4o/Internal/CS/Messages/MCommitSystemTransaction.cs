namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MCommitSystemTransaction : Db4objects.Db4o.Internal.CS.Messages.Msg, 
		Db4objects.Db4o.Internal.CS.Messages.IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			Transaction().SystemTransaction().Commit();
			return true;
		}
	}
}
