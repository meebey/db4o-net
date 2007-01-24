namespace Db4objects.Db4o.Foundation.Network
{
	public class YapServerSocket
	{
		private Sharpen.Net.ServerSocket _serverSocket;

		public YapServerSocket(int port)
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

		public virtual Db4objects.Db4o.Foundation.Network.IYapSocket Accept()
		{
			Sharpen.Net.Socket sock = _serverSocket.Accept();
			return new Db4objects.Db4o.Foundation.Network.YapSocketReal(sock);
		}

		public virtual void Close()
		{
			_serverSocket.Close();
		}
	}
}
