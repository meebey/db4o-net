namespace Db4objects.Db4o.Internal.CS.Messages
{
	public class MWriteBatchedMessages : Db4objects.Db4o.Internal.CS.Messages.MsgD
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
			 serverThread)
		{
			int count = ReadInt();
			Db4objects.Db4o.Internal.Transaction ta = Transaction();
			for (int i = 0; i < count; i++)
			{
				Db4objects.Db4o.Internal.StatefulBuffer writer = _payLoad.ReadYapBytes();
				int messageId = writer.ReadInt();
				Db4objects.Db4o.Internal.CS.Messages.Msg message = Db4objects.Db4o.Internal.CS.Messages.Msg
					.GetMessage(messageId);
				Db4objects.Db4o.Internal.CS.Messages.Msg clonedMessage = message.Clone(ta);
				if (clonedMessage is Db4objects.Db4o.Internal.CS.Messages.MsgD)
				{
					Db4objects.Db4o.Internal.CS.Messages.MsgD mso = (Db4objects.Db4o.Internal.CS.Messages.MsgD
						)clonedMessage;
					mso.PayLoad(writer);
					if (mso.PayLoad() != null)
					{
						mso.PayLoad().IncrementOffset(Db4objects.Db4o.Internal.Const4.MESSAGE_LENGTH - Db4objects.Db4o.Internal.Const4
							.INT_LENGTH);
						mso.PayLoad().SetTransaction(ta);
						mso.ProcessAtServer(serverThread);
					}
				}
				else
				{
					if (!clonedMessage.ProcessAtServer(serverThread))
					{
						serverThread.ProcessSpecialMsg(clonedMessage);
					}
				}
			}
			return true;
		}
	}
}
