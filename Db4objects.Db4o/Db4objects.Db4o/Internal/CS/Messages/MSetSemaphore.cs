using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MSetSemaphore : MsgD, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			int timeout = ReadInt();
			string name = ReadString();
			LocalObjectContainer stream = (LocalObjectContainer)Stream();
			bool res = stream.SetSemaphore(Transaction(), name, timeout);
			if (res)
			{
				Write(Msg.SUCCESS);
			}
			else
			{
				Write(Msg.FAILED);
			}
			return true;
		}
	}
}
