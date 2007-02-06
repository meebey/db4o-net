namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MWriteUpdateDeleteMembers : Db4objects.Db4o.Internal.CS.Messages.MsgD
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
			 serverThread)
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
