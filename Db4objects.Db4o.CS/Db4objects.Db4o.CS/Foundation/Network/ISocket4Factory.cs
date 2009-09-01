/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation.Network;

namespace Db4objects.Db4o.Foundation.Network
{
	public interface ISocket4Factory
	{
		/// <exception cref="System.IO.IOException"></exception>
		ISocket4 CreateSocket(string hostName, int port);

		/// <exception cref="System.IO.IOException"></exception>
		IServerSocket4 CreateServerSocket(int port);
	}
}
