/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Internal.Messages;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.CS.Internal.Messages
{
	public sealed class MPrefetchIDs : MsgD, IMessageWithResponse
	{
		public Msg ReplyFromServer()
		{
			int prefetchIDCount = ReadInt();
			MsgD reply = Msg.IdList.GetWriterForLength(Transaction(), Const4.IntLength * prefetchIDCount
				);
			lock (StreamLock())
			{
				for (int i = 0; i < prefetchIDCount; i++)
				{
					reply.WriteInt(((LocalObjectContainer)Stream()).PrefetchID());
				}
			}
			return reply;
		}
	}
}
