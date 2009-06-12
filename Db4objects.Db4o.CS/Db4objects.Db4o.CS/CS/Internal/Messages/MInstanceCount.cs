/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Internal.Messages;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.CS.Internal.Messages
{
	public class MInstanceCount : MsgD, IMessageWithResponse
	{
		public virtual bool ProcessAtServer()
		{
			MsgD writer = null;
			lock (StreamLock())
			{
				ClassMetadata clazz = File().ClassMetadataForID(ReadInt());
				writer = Msg.InstanceCount.GetWriterForInt(Transaction(), clazz.IndexEntryCount(Transaction
					()));
			}
			Write(writer);
			return true;
		}
	}
}
