/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public class MInstanceCount : MsgD, IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			MsgD writer = null;
			lock (StreamLock())
			{
				ClassMetadata clazz = File().ClassMetadataForId(ReadInt());
				writer = Msg.InstanceCount.GetWriterForInt(Transaction(), clazz.IndexEntryCount(Transaction
					()));
			}
			Write(writer);
			return true;
		}
	}
}
