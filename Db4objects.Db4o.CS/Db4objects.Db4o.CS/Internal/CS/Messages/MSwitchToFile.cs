/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MSwitchToFile : MsgD, IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			IServerMessageDispatcher serverThread = ServerMessageDispatcher();
			serverThread.SwitchToFile(this);
			return true;
		}
	}
}
