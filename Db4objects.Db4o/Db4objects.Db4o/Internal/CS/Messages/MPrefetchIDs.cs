/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MPrefetchIDs : MsgD, IServerSideMessage
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
