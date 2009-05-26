/* Copyright (C) 2009  Versant Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.CS.Internal
{
	class ServerPlatform
	{
		internal static void TriggerMessageEvent(System.EventHandler<MessageEventArgs> eventHandler, Db4objects.Db4o.CS.Internal.Messages.Msg message)
		{
			if (eventHandler == null)
				return;

			eventHandler(null, new MessageEventArgs(message));
		}
	}
}
