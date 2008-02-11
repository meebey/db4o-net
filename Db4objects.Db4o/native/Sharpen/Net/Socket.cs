/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;
using System.IO;
using System.Net;
using System.Threading;
using Sharpen.IO;
using NativeSocket=System.Net.Sockets.Socket;
using System.Net.Sockets;

namespace Sharpen.Net
{
	public class Socket : SocketWrapper
	{	
		IInputStream _in;
		IOutputStream _out;

		public Socket(string hostName, int port)
		{
		    NativeSocket socket = new NativeSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.Connect(new IPEndPoint(Resolve(hostName), port));
			Initialize(socket);
		}

	    private static IPAddress Resolve(string hostName)
	    {
	    	IPHostEntry found = Dns.GetHostEntry(hostName);
	        foreach (IPAddress address in found.AddressList)
	        {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    return address;
                }
	        }
	        throw new IOException("couldn't find suitable address for name '" + hostName + "'");
	    }

	    public Socket(NativeSocket socket)
		{
			Initialize(socket);
		}

		public IInputStream GetInputStream()
		{
			return _in;
		}

		public IOutputStream GetOutputStream()
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

#if CF
			_in = new SocketInputStream(this);
#else
			_in = new InputStream(stream);
#endif
			_out = new OutputStream(stream);
		}
	}
#if CF
	internal class SocketInputStream : IInputStream
    {
    	private readonly Socket _socket;

    	public SocketInputStream(Socket socket)
        {
    		_socket = socket;
        }

    	public int Read()
    	{
			byte[] buffer = new byte[1];
    		if (1 != Read(buffer))
    		{
    			return -1;
    		}
    		return (int) buffer[0];
    	}

    	public int Read(byte[] bytes)
    	{
    		return Read(bytes, 0, bytes.Length);
    	}

    	public int Read(byte[] bytes, int offset, int length)
    	{
			try
			{
				if (_socket.SoTimeout > 0)
				{
					if (!UnderlyingSocket.Poll(_socket.SoTimeout*1000, SelectMode.SelectRead))
					{
						throw new IOException("read timeout");
					}
				}
				return InputStream.TranslateReadReturnValue(
					UnderlyingSocket.Receive(bytes, offset, length, SocketFlags.None));
			}
			catch (ObjectDisposedException x)
			{
				throw new IOException(x.Message, x);
			}
			catch (SocketException x)
			{
				throw new IOException(x.Message, x);
			}
    	}

    	public void Close()
    	{
    		// nothing to do
    	}

    	private System.Net.Sockets.Socket UnderlyingSocket
    	{
			get { return _socket.UnderlyingSocket;  }
    	}
    }
#endif
}
