using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MCommitSystemTransaction : Msg, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			Transaction().SystemTransaction().Commit();
			return true;
		}
	}
}
