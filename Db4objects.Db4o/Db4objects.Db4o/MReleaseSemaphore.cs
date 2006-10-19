namespace Db4objects.Db4o
{
	internal sealed class MReleaseSemaphore : Db4objects.Db4o.MsgD
	{
		internal sealed override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 sock)
		{
			string name = ReadString();
			((Db4objects.Db4o.YapFile)GetStream()).ReleaseSemaphore(GetTransaction(), name);
			return true;
		}
	}
}
