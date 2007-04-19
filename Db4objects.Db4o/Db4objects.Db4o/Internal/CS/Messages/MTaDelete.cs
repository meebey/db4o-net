using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public class MTaDelete : MsgD, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			int id = _payLoad.ReadInt();
			int cascade = _payLoad.ReadInt();
			Transaction trans = Transaction();
			lock (StreamLock())
			{
				trans.Delete(null, id, cascade);
				return true;
			}
		}
	}
}
