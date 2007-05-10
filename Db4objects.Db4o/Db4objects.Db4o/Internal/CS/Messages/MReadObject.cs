using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MReadObject : MsgD, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			StatefulBuffer bytes = null;
			lock (StreamLock())
			{
				try
				{
					bytes = Stream().ReadWriterByID(Transaction(), _payLoad.ReadInt());
				}
				catch (Db4oException e)
				{
					WriteException(e);
					return true;
				}
			}
			if (bytes == null)
			{
				bytes = new StatefulBuffer(Transaction(), 0, 0);
			}
			Write(Msg.OBJECT_TO_CLIENT.GetWriter(bytes));
			return true;
		}
	}
}
