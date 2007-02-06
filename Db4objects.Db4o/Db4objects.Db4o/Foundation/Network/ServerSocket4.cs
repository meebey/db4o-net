namespace Db4objects.Db4o.Foundation.Network
{
	public class ServerSocket4
	{
		private Sharpen.Net.ServerSocket _serverSocket;

		public ServerSocket4(int port)
		{
			_serverSocket = new Sharpen.Net.ServerSocket(port);
		}

		public virtual void SetSoTimeout(int timeout)
		{
			try
			{
				_serverSocket.SetSoTimeout(timeout);
			}
			catch (System.Net.Sockets.SocketException e)
			{
				Sharpen.Runtime.PrintStackTrace(e);
			}
		}

		public virtual int GetLocalPort()
		{
			return _serverSocket.GetLocalPort();
		}

		public virtual Db4objects.Db4o.Foundation.Network.ISocket4 Accept()
		{
			Sharpen.Net.Socket sock = _serverSocket.Accept();
			return new Db4objects.Db4o.Foundation.Network.NetworkSocket(sock);
		}

		public virtual void Close()
		{
			_serverSocket.Close();
		}
	}
}
