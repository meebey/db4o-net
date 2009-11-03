/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation.Network;

namespace Db4objects.Db4o.Foundation.Network
{
	public class StandardSocket4Factory : ISocket4Factory
	{
		/// <exception cref="System.IO.IOException"></exception>
		public virtual IServerSocket4 CreateServerSocket(int port)
		{
			return new NetworkServerSocket(port);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual ISocket4 CreateSocket(string hostName, int port)
		{
			return new NetworkSocket(hostName, port);
		}
	}
}
