using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MReleaseSemaphore : MsgD, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			string name = ReadString();
			((LocalObjectContainer)Stream()).ReleaseSemaphore(Transaction(), name);
			return true;
		}
	}
}
