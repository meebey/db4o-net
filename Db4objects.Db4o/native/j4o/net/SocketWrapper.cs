using System.Net.Sockets;
using NativeSocket=System.Net.Sockets.Socket;

namespace Sharpen.Net
{
	public class SocketWrapper
	{
		protected NativeSocket _delegate;

		protected virtual void Initialize(NativeSocket socket)
		{
			_delegate = socket;
		}

		public void SetSoTimeout(int timeout)
		{
            // Socket timeouts have turned out to be a bad option on all supported
            // .NET platforms. If timeouts are turned on, the waiting message loop
            // will run into IoExceptions, causing a CPU load of 100%. 

            // All test cases work fine without socket timeouts on the server.

//			_delegate.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, timeout);
//			_delegate.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, timeout);
		}

		public void Close()
		{
			_delegate.Close();
		}

        public bool IsConnected() 
        {
            return _delegate.Connected;
        }

	}
}
