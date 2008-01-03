/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

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
			lock (StreamLock())
			{
				for (int i = 0; i < count; i++)
				{
					StatefulBuffer writer = _payLoad.ReadYapBytes();
					int messageId = writer.ReadInt();
					Msg message = Msg.GetMessage(messageId);
					Msg clonedMessage = message.PublicClone();
					clonedMessage.SetMessageDispatcher(MessageDispatcher());
					clonedMessage.SetTransaction(ta);
					if (clonedMessage is MsgD)
					{
						MsgD msgd = (MsgD)clonedMessage;
						msgd.PayLoad(writer);
						if (msgd.PayLoad() != null)
						{
							msgd.PayLoad().IncrementOffset(Const4.IntLength);
							Transaction t = CheckParentTransaction(ta, msgd.PayLoad());
							msgd.SetTransaction(t);
							((IServerSideMessage)msgd).ProcessAtServer();
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
}
