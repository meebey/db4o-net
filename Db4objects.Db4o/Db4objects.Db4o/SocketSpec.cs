/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Config;

namespace Db4objects.Db4o
{
	/// <summary>Specifies a socket connection via a socket factory and a port number.</summary>
	/// <remarks>Specifies a socket connection via a socket factory and a port number.</remarks>
	/// <exclude></exclude>
	public class SocketSpec
	{
		private readonly int _port;

		private readonly INativeSocketFactory _factory;

		public SocketSpec(int port, INativeSocketFactory factory)
		{
			_port = port;
			_factory = factory;
		}

		public virtual int Port()
		{
			return _port;
		}

		public virtual INativeSocketFactory SocketFactory()
		{
			return _factory;
		}
	}
}
