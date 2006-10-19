namespace Db4objects.Db4o
{
	internal sealed class MSetSemaphore : Db4objects.Db4o.MsgD
	{
		internal sealed override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 sock)
		{
			int timeout = ReadInt();
			string name = ReadString();
			Db4objects.Db4o.YapFile stream = (Db4objects.Db4o.YapFile)GetStream();
			bool res = stream.SetSemaphore(GetTransaction(), name, timeout);
			if (res)
			{
				Db4objects.Db4o.Msg.SUCCESS.Write(stream, sock);
			}
			else
			{
				Db4objects.Db4o.Msg.FAILED.Write(stream, sock);
			}
			return true;
		}
	}
}
