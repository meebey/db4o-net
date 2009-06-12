/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Internal.Messages;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.CS.Internal.Messages
{
	public class MReadReaderById : MsgD, IMessageWithResponse
	{
		public bool ProcessAtServer()
		{
			ByteArrayBuffer bytes = null;
			// readWriterByID may fail in certain cases, for instance if
			// and object was deleted by another client
			lock (StreamLock())
			{
				bytes = Stream().ReadReaderByID(Transaction(), _payLoad.ReadInt(), _payLoad.ReadInt
					() == 1);
			}
			if (bytes == null)
			{
				bytes = new ByteArrayBuffer(0);
			}
			Write(Msg.ReadBytes.GetWriter(Transaction(), bytes));
			return true;
		}
	}
}
