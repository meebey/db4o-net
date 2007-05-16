/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;
using System.Net;
using System.Net.Sockets;
using NativeSocket=System.Net.Sockets.Socket;

namespace Sharpen.Net
{
	public class ServerSocket : SocketWrapper
	{
		public ServerSocket(int port)
		{
            try
            {
                NativeSocket socket = new NativeSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(new IPEndPoint(IPAddress.Any, port));

                int maxPendingConnections = 42;
                socket.Listen(maxPendingConnections);
                Initialize(socket);
            }
            catch (SocketException e)
            {
                throw new System.IO.IOException(e.Message);
            }

		}

		public Socket Accept()
		{
			return new Socket(_delegate.Accept());
		}

		public int GetLocalPort()
		{
			return ((IPEndPoint)_delegate.LocalEndPoint).Port;
		}
	}
}
