using System;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MDelete : MsgD, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			Db4objects.Db4o.Internal.Buffer bytes = this.GetByteLoad();
			ObjectContainerBase stream = Stream();
			lock (StreamLock())
			{
				object obj = stream.GetByID1(Transaction(), bytes.ReadInt());
				bool userCall = bytes.ReadInt() == 1;
				if (obj != null)
				{
					try
					{
						stream.Delete1(Transaction(), obj, userCall);
					}
					catch (Exception)
					{
					}
				}
			}
			return true;
		}
	}
}
