/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MClose : Msg, IServerSideMessage, IClientSideMessage
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
				ServerMessageDispatcher().Close();
			}
			return true;
		}

		public virtual bool ProcessAtClient()
		{
			ClientMessageDispatcher().Close();
			return true;
		}
	}
}
