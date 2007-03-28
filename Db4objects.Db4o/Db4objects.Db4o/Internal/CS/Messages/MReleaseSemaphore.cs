namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MReleaseSemaphore : Db4objects.Db4o.Internal.CS.Messages.MsgD
		, Db4objects.Db4o.Internal.CS.Messages.IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			string name = ReadString();
			((Db4objects.Db4o.Internal.LocalObjectContainer)Stream()).ReleaseSemaphore(Transaction
				(), name);
			return true;
		}
	}
}
