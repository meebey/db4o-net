/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.Net.Sockets;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Foundation.Network;
using Sharpen.Net;

namespace Db4objects.Db4o.Foundation.Network
{
	public class ServerSocket4
	{
		private ServerSocket _serverSocket;

		private INativeSocketFactory _factory;

		public ServerSocket4(INativeSocketFactory factory, int port)
		{
			_factory = factory;
			_serverSocket = _factory.CreateServerSocket(port);
		}

		public virtual void SetSoTimeout(int timeout)
		{
			try
			{
				_serverSocket.SetSoTimeout(timeout);
			}
			catch (SocketException e)
			{
				Sharpen.Runtime.PrintStackTrace(e);
			}
		}

		public virtual int GetLocalPort()
		{
			return _serverSocket.GetLocalPort();
		}

		public virtual ISocket4 Accept()
		{
			Sharpen.Net.Socket sock = _serverSocket.Accept();
			return new NetworkSocket(_factory, sock);
		}

		public virtual void Close()
		{
			_serverSocket.Close();
		}
	}
}
