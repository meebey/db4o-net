namespace Db4objects.Db4o.CS.Messages
{
	public sealed class MWriteUpdateDeleteMembers : Db4objects.Db4o.CS.Messages.MsgD
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.CS.YapServerThread serverThread
			)
		{
			lock (StreamLock())
			{
				Transaction().WriteUpdateDeleteMembers(ReadInt(), Stream().GetYapClass(ReadInt())
					, ReadInt(), ReadInt());
			}
			return true;
		}
	}
}
