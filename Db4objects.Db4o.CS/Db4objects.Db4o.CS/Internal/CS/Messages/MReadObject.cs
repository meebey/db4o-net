/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MReadObject : MsgD, IMessageWithResponse
	{
		public bool ProcessAtServer()
		{
			StatefulBuffer bytes = null;
			// readWriterByID may fail in certain cases, for instance if
			// and object was deleted by another client
			lock (StreamLock())
			{
				bytes = Stream().ReadWriterByID(Transaction(), _payLoad.ReadInt(), _payLoad.ReadInt
					() == 1);
			}
			if (bytes == null)
			{
				bytes = new StatefulBuffer(Transaction(), 0, 0);
			}
			Write(Msg.ObjectToClient.GetWriter(bytes));
			return true;
		}
	}
}
