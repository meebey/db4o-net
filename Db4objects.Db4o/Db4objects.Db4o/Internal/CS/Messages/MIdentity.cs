namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MIdentity : Db4objects.Db4o.Internal.CS.Messages.Msg, Db4objects.Db4o.Internal.CS.Messages.IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			Db4objects.Db4o.Internal.ObjectContainerBase stream = Stream();
			RespondInt((int)stream.GetID(stream.Identity()));
			return true;
		}
	}
}
