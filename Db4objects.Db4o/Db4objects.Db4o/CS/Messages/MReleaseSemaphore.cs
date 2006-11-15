namespace Db4objects.Db4o.CS.Messages
{
	public sealed class MReleaseSemaphore : Db4objects.Db4o.CS.Messages.MsgD
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.CS.YapServerThread serverThread
			)
		{
			string name = ReadString();
			((Db4objects.Db4o.YapFile)Stream()).ReleaseSemaphore(Transaction(), name);
			return true;
		}
	}
}
