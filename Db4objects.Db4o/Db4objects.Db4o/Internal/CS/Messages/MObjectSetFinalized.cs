namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MObjectSetFinalized : Db4objects.Db4o.Internal.CS.Messages.MsgD, Db4objects.Db4o.Internal.CS.Messages.IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			int queryResultID = ReadInt();
			ServerMessageDispatcher().QueryResultFinalized(queryResultID);
			return true;
		}
	}
}
