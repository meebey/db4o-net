namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MObjectSetReset : Db4objects.Db4o.Internal.CS.Messages.MObjectSet, Db4objects.Db4o.Internal.CS.Messages.IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			Stub(ReadInt()).Reset();
			return true;
		}
	}
}
