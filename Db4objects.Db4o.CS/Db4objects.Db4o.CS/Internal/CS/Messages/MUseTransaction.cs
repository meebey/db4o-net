/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MUseTransaction : MsgD, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			IServerMessageDispatcher serverThread = ServerMessageDispatcher();
			serverThread.UseTransaction(this);
			return true;
		}
	}
}
