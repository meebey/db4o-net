namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MObjectSetReset : Db4objects.Db4o.Internal.CS.Messages.MObjectSet
	{
		public override bool ProcessAtServer(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
			 serverThread)
		{
			Stub(serverThread, ReadInt()).Reset();
			return true;
		}
	}
}
