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
            IPEndPoint ipEndPoint = new IPEndPoint(Dns.Resolve(hostName).AddressList[0], port);
            NativeSocket socket = new NativeSocket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipEndPoint);
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
			return _socket.UnderlyingSocket.Poll(timeout, SelectMode.SelectRead);
    	}

    	private int GetMicroSecondsTimeout()
    	{
    		return _socket.SoTimeout*1000;
    	}
    }
}
