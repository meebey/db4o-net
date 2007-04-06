using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public class MWriteBatchedMessages : MsgD, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			int count = ReadInt();
			Transaction ta = Transaction();
			for (int i = 0; i < count; i++)
			{
				StatefulBuffer writer = _payLoad.ReadYapBytes();
				int messageId = writer.ReadInt();
				Msg message = Msg.GetMessage(messageId);
				Msg clonedMessage = message.PublicClone();
				clonedMessage.SetTransaction(ta);
				if (clonedMessage is MsgD)
				{
					MsgD mso = (MsgD)clonedMessage;
					mso.PayLoad(writer);
					if (mso.PayLoad() != null)
					{
						mso.PayLoad().IncrementOffset(Const4.MESSAGE_LENGTH - Const4.INT_LENGTH);
						mso.PayLoad().SetTransaction(ta);
						((IServerSideMessage)mso).ProcessAtServer();
					}
				}
				else
				{
					((IServerSideMessage)clonedMessage).ProcessAtServer();
				}
			}
			return true;
		}
	}
}
