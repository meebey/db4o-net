/* Copyright (C) 2009  Versant Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.CS.Internal
{
	class ServerPlatform
	{
		internal static void TriggerMessageEvent(System.EventHandler<MessageEventArgs> eventHandler, Db4objects.Db4o.CS.Internal.Messages.IMessage message)
		{
			if (eventHandler == null)
				return;

			eventHandler(null, new MessageEventArgs(message));
		}

		internal static void TriggerClientConnectionEvent(System.EventHandler<ClientConnectionEventArgs> eventHandler, Db4objects.Db4o.CS.Internal.IClientConnection connection)
		{
			if (eventHandler == null)
				return;

			eventHandler(null, new ClientConnectionEventArgs(connection));
		}
	}
}
