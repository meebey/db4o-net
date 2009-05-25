/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MTaIsDeleted : MsgD, IMessageWithResponse
	{
		public bool ProcessAtServer()
		{
			lock (StreamLock())
			{
				bool isDeleted = Transaction().IsDeleted(ReadInt());
				int ret = isDeleted ? 1 : 0;
				Write(Msg.TaIsDeleted.GetWriterForInt(Transaction(), ret));
			}
			return true;
		}
	}
}
