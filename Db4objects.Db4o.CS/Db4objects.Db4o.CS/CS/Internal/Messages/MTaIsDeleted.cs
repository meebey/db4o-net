/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Internal.Messages;

namespace Db4objects.Db4o.CS.Internal.Messages
{
	public sealed class MTaIsDeleted : MsgD, IMessageWithResponse
	{
		public Msg ReplyFromServer()
		{
			lock (StreamLock())
			{
				bool isDeleted = Transaction().IsDeleted(ReadInt());
				int ret = isDeleted ? 1 : 0;
				return Msg.TaIsDeleted.GetWriterForInt(Transaction(), ret);
			}
		}
	}
}
