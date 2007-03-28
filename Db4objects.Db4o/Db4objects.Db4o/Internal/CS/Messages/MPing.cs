namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MPing : Db4objects.Db4o.Internal.CS.Messages.Msg, Db4objects.Db4o.Internal.CS.Messages.IServerSideMessage
		, Db4objects.Db4o.Internal.CS.Messages.IClientSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			Write(Db4objects.Db4o.Internal.CS.Messages.Msg.OK);
			return true;
		}

		public virtual bool ProcessAtClient()
		{
			Write(Db4objects.Db4o.Internal.CS.Messages.Msg.OK);
			return true;
		}
	}
}
