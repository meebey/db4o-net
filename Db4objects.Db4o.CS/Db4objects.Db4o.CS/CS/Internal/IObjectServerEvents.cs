/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.CS.Internal
{
	public interface IObjectServerEvents
	{
		event System.EventHandler<ClientConnectionEventArgs> ClientConnected;

		event System.EventHandler<ServerClosedEventArgs> Closed;
	}
}
