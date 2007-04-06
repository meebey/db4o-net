using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MCommit : Msg, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			try
			{
				ServerTransaction().Commit(ServerMessageDispatcher());
				Write(Msg.OK);
			}
			catch (Db4oException e)
			{
				WriteException(e);
			}
			return true;
		}
	}
}
