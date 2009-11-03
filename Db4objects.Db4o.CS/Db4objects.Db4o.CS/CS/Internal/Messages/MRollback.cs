/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Internal.Messages;

namespace Db4objects.Db4o.CS.Internal.Messages
{
	public sealed class MRollback : Msg, IServerSideMessage
	{
		public void ProcessAtServer()
		{
			lock (StreamLock())
			{
				Transaction().Rollback();
			}
		}
	}
}
