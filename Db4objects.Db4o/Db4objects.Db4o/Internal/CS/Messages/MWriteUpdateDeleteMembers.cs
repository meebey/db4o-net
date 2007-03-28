namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MWriteUpdateDeleteMembers : Db4objects.Db4o.Internal.CS.Messages.MsgD
		, Db4objects.Db4o.Internal.CS.Messages.IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			lock (StreamLock())
			{
				Transaction().WriteUpdateDeleteMembers(ReadInt(), Stream().ClassMetadataForId(ReadInt
					()), ReadInt(), ReadInt());
			}
			return true;
		}
	}
}
