/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;
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
