using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MRollback : Msg, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			Transaction().Rollback();
			return true;
		}
	}
}
