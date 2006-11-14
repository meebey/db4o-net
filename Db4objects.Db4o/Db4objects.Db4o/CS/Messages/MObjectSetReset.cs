namespace Db4objects.Db4o.CS.Messages
{
	/// <exclude></exclude>
	public class MObjectSetReset : Db4objects.Db4o.CS.Messages.MObjectSet
	{
		public override bool ProcessAtServer(Db4objects.Db4o.CS.YapServerThread serverThread
			)
		{
			Stub(serverThread, ReadInt()).Reset();
			return true;
		}
	}
}
