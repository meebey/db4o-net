namespace Db4objects.Db4o.CS.Messages
{
	public class MWriteBatchedMessages : Db4objects.Db4o.CS.Messages.MsgD
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.CS.YapServerThread serverThread
			)
		{
			int count = ReadInt();
			Db4objects.Db4o.Transaction ta = Transaction();
			for (int i = 0; i < count; i++)
			{
				Db4objects.Db4o.YapWriter writer = _payLoad.ReadYapBytes();
				int messageId = writer.ReadInt();
				Db4objects.Db4o.CS.Messages.Msg message = Db4objects.Db4o.CS.Messages.Msg.GetMessage
					(messageId);
				Db4objects.Db4o.CS.Messages.Msg clonedMessage = message.Clone(ta);
				if (clonedMessage is Db4objects.Db4o.CS.Messages.MsgD)
				{
					Db4objects.Db4o.CS.Messages.MsgD mso = (Db4objects.Db4o.CS.Messages.MsgD)clonedMessage;
					mso.PayLoad(writer);
					if (mso.PayLoad() != null)
					{
						mso.PayLoad().IncrementOffset(Db4objects.Db4o.YapConst.MESSAGE_LENGTH - Db4objects.Db4o.YapConst
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
