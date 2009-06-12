/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Internal.Messages;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.CS.Internal.Messages
{
	public sealed class MPrefetchIDs : MsgD, IMessageWithResponse
	{
		public bool ProcessAtServer()
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
			Write(reply);
			return true;
		}
	}
}
