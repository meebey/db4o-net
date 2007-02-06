namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MReleaseSemaphore : Db4objects.Db4o.Internal.CS.Messages.MsgD
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
			 serverThread)
		{
			string name = ReadString();
			((Db4objects.Db4o.Internal.LocalObjectContainer)Stream()).ReleaseSemaphore(Transaction
				(), name);
			return true;
		}
	}
}
