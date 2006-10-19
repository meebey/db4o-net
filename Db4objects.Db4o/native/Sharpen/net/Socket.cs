using System;
using System.Net;
using Sharpen.IO;
using NativeSocket=System.Net.Sockets.Socket;
using System.Net.Sockets;

namespace Sharpen.Net
{
	public class Socket : SocketWrapper
	{	
		InputStream _in;
		OutputStream _out;

		public Socket(string hostName, int port)
		{
			NativeSocket socket = new NativeSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.Connect(new IPEndPoint(Dns.Resolve(hostName).AddressList[0], port));
			Initialize(socket);
		}

		public Socket(NativeSocket socket)
		{
			Initialize(socket);
		}

		public InputStream GetInputStream()
		{
			return _in;
		}

		public OutputStream GetOutputStream()
		{
			return _out;
		}

		public int GetPort() 
		{
			return ((IPEndPoint)base._delegate.RemoteEndPoint).Port;
		}

		override protected void Initialize(NativeSocket socket)
		{
			base.Initialize(socket);
			NetworkStream stream = new NetworkStream(_delegate);
			_in = new InputStream(stream);
			_out = new OutputStream(stream);
		}
	}
}
