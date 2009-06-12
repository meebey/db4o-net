/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Internal.Messages;

namespace Db4objects.Db4o.CS.Internal.Messages
{
	/// <exclude></exclude>
	public class MCloseSocket : Msg, IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			lock (Stream().Lock())
			{
				if (Stream().IsClosed())
				{
					return true;
				}
				Transaction().Commit();
				LogMsg(35, ServerMessageDispatcher().Name());
				ServerMessageDispatcher().CloseConnection();
			}
			return true;
		}
	}
}
