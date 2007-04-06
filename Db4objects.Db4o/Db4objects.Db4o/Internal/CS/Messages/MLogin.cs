using Db4objects.Db4o;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MLogin : MsgD, IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			string userName = ReadString();
			string password = ReadString();
			ObjectServerImpl server = ServerMessageDispatcher().Server();
			User found = server.GetUser(userName);
			if (found != null)
			{
				if (found.password.Equals(password))
				{
					ServerMessageDispatcher().SetDispatcherName(userName);
					ServerMessageDispatcher().Login();
					LogMsg(32, userName);
					int blockSize = Stream().BlockSize();
					int encrypt = Stream().i_handlers.i_encrypt ? 1 : 0;
					Write(Msg.LOGIN_OK.GetWriterForInts(Transaction(), new int[] { blockSize, encrypt
						 }));
					return true;
				}
			}
			Write(Msg.FAILED);
			return true;
		}
	}
}
