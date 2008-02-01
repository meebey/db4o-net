/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MDelete : MsgD, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			ByteArrayBuffer bytes = this.GetByteLoad();
			ObjectContainerBase stream = Stream();
			lock (StreamLock())
			{
				object obj = stream.GetByID(Transaction(), bytes.ReadInt());
				bool userCall = bytes.ReadInt() == 1;
				if (obj != null)
				{
					try
					{
						stream.Delete1(Transaction(), obj, userCall);
					}
					catch (Exception e)
					{
					}
				}
			}
			return true;
		}
	}
}
