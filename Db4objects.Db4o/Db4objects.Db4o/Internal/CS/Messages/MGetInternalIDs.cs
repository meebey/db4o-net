using System;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MGetInternalIDs : MsgD, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			Db4objects.Db4o.Internal.Buffer bytes = this.GetByteLoad();
			long[] ids;
			lock (StreamLock())
			{
				try
				{
					ids = Stream().ClassMetadataForId(bytes.ReadInt()).GetIDs(Transaction());
				}
				catch (Exception)
				{
					ids = new long[0];
				}
			}
			int size = ids.Length;
			MsgD message = Msg.ID_LIST.GetWriterForLength(Transaction(), Const4.ID_LENGTH * (
				size + 1));
			Db4objects.Db4o.Internal.Buffer writer = message.PayLoad();
			writer.WriteInt(size);
			for (int i = 0; i < size; i++)
			{
				writer.WriteInt((int)ids[i]);
			}
			Write(message);
			return true;
		}
	}
}
