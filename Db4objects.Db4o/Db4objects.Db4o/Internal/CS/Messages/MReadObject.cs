/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MReadObject : MsgD, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			StatefulBuffer bytes = null;
			// readObjectByID may fail in certain cases, for instance if
			// and object was deleted by another client
			lock (StreamLock())
			{
				try
				{
					bytes = Stream().ReadWriterByID(Transaction(), _payLoad.ReadInt(), _payLoad.ReadInt
						() == 1);
				}
				catch (Db4oException e)
				{
					WriteException(e);
					return true;
				}
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
