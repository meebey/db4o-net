namespace Db4objects.Db4o.CS.Messages
{
	public sealed class MReleaseSemaphore : Db4objects.Db4o.CS.Messages.MsgD
	{
		public sealed override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 sock)
		{
			string name = ReadString();
			((Db4objects.Db4o.YapFile)GetStream()).ReleaseSemaphore(GetTransaction(), name);
			return true;
		}
	}
}
