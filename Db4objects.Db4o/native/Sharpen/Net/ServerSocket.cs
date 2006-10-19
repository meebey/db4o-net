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
			NativeSocket socket = new NativeSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.Bind(new IPEndPoint(IPAddress.Any, port));

			int maxConnections = 42;
			socket.Listen(maxConnections);

			Initialize(socket);
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
