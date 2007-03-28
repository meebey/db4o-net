namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MLogin : Db4objects.Db4o.Internal.CS.Messages.MsgD, Db4objects.Db4o.Internal.CS.Messages.IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			string userName = ReadString();
			string password = ReadString();
			Db4objects.Db4o.Internal.CS.ObjectServerImpl server = ServerMessageDispatcher().Server
				();
			Db4objects.Db4o.User found = server.GetUser(userName);
			if (found != null)
			{
				if (found.password.Equals(password))
				{
					ServerMessageDispatcher().SetDispatcherName(userName);
					ServerMessageDispatcher().Login();
					LogMsg(32, userName);
					int blockSize = Stream().BlockSize();
					int encrypt = Stream().i_handlers.i_encrypt ? 1 : 0;
					Write(Db4objects.Db4o.Internal.CS.Messages.Msg.LOGIN_OK.GetWriterForInts(Transaction
						(), new int[] { blockSize, encrypt }));
					return true;
				}
			}
			Write(Db4objects.Db4o.Internal.CS.Messages.Msg.FAILED);
			return true;
		}
	}
}
