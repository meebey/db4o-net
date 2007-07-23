/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;
using System.Collections;
using System.IO;
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
			socket.Connect(new IPEndPoint(Resolve(hostName), port));
			Initialize(socket);
		}

	    private static IPAddress Resolve(string hostName)
	    {
	        IPHostEntry found = Dns.Resolve(hostName);
#if !CF_1_0
	        foreach (IPAddress address in found.AddressList)
	        {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    return address;
                }
	        }
	        throw new IOException("couldn't find suitable address for name '" + hostName + "'");
#else
            return found.AddressList[0];
#endif
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
			_in = new SocketInputStream(stream, this);
			_out = new OutputStream(stream);
		}
	}

    internal class SocketInputStream : InputStream
    {
    	private Socket _socket;

    	public SocketInputStream(NetworkStream stream, Socket socket) : base(stream)
        {
    		_socket = socket;
        }

		protected override void BeforeRead()
		{
			if (!IsTimeoutConfigured())
			{
				return;
			}
			if (!PollRead(GetMicroSecondsTimeout()))
			{
				throw new IOException("timeout");
			}
		}

    	private bool IsTimeoutConfigured()
    	{
    		return _socket.SoTimeout != 0;
    	}

    	private bool PollRead(int timeout)
    	{
#if CF_1_0 || CF_2_0
            try
            {
                return _socket.UnderlyingSocket.Poll(timeout, SelectMode.SelectRead);
            }
            catch (SocketException e)
            {
                return false;
            }
#else
            return _socket.UnderlyingSocket.Poll(timeout, SelectMode.SelectRead);
#endif
        }

    	private int GetMicroSecondsTimeout()
    	{
    		return _socket.SoTimeout*1000;
    	}
    }
}
