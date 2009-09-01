/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System.Net.Sockets;
using Db4objects.Db4o.Foundation.Network;
using Sharpen.Net;

namespace Db4objects.Db4o.Foundation.Network
{
	public class NetworkServerSocket : IServerSocket4
	{
		private ServerSocket _serverSocket;

		/// <exception cref="System.IO.IOException"></exception>
		public NetworkServerSocket(int port)
		{
			_serverSocket = CreateServerSocket(port);
		}

		/// <exception cref="System.IO.IOException"></exception>
		protected virtual ServerSocket CreateServerSocket(int port)
		{
			return new ServerSocket(port);
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

		/// <exception cref="System.IO.IOException"></exception>
		public virtual ISocket4 Accept()
		{
			Sharpen.Net.Socket sock = _serverSocket.Accept();
			// TODO: check connection permissions here
			return new NetworkSocket(sock);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Close()
		{
			_serverSocket.Close();
		}
	}
}
