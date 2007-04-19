using System.Net.Sockets;
using Db4objects.Db4o.Foundation.Network;
using Sharpen.Net;

namespace Db4objects.Db4o.Foundation.Network
{
	public class ServerSocket4
	{
		private ServerSocket _serverSocket;

		public ServerSocket4(int port)
		{
			_serverSocket = new ServerSocket(port);
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
			return new NetworkSocket(sock);
		}

		public virtual void Close()
		{
			_serverSocket.Close();
		}
	}
}
